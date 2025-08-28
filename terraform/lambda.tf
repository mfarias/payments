# Lambda function
resource "aws_lambda_function" "api" {
  filename         = "payments-api.zip"
  function_name    = "${var.project_name}-api"
  role            = aws_iam_role.lambda_execution_role.arn
  handler         = "Payments.Api::Payments.Api.LambdaEntryPoint::FunctionHandlerAsync"
  runtime         = "dotnet8"
  memory_size     = var.lambda_memory
  timeout         = var.lambda_timeout
  
  environment {
    variables = {
      ConnectionStrings__DefaultConnection = "Host=${aws_db_instance.main.endpoint};Database=${aws_db_instance.main.db_name};Username=${aws_db_instance.main.username};Password=${random_password.db_password.result};Port=${aws_db_instance.main.port};"
      ASPNETCORE_ENVIRONMENT = "Production"
    }
  }

  depends_on = [aws_cloudwatch_log_group.lambda]

  tags = {
    Name = "${var.project_name}-lambda"
  }
}

# CloudWatch Log Group
resource "aws_cloudwatch_log_group" "lambda" {
  name              = "/aws/lambda/${var.project_name}-api"
  retention_in_days = 7

  tags = {
    Name = "${var.project_name}-lambda-logs"
  }
}

# Provisioned Concurrency (prevents cold starts)
resource "aws_lambda_provisioned_concurrency_config" "api" {
  count                             = var.enable_provisioned_concurrency ? 1 : 0
  function_name                     = aws_lambda_function.api.function_name
  provisioned_concurrency_capacity = 1
  qualifier                        = aws_lambda_function.api.version
}

# API Gateway HTTP API
resource "aws_apigatewayv2_api" "main" {
  name          = "${var.project_name}-api"
  protocol_type = "HTTP"
  description   = "HTTP API for ${var.project_name}"

  cors_configuration {
    allow_credentials = false
    allow_headers     = ["*"]
    allow_methods     = ["*"]
    allow_origins     = ["*"]
    max_age          = 86400
  }

  tags = {
    Name = "${var.project_name}-api-gateway"
  }
}

# Lambda integration
resource "aws_apigatewayv2_integration" "lambda" {
  api_id           = aws_apigatewayv2_api.main.id
  integration_type = "AWS_PROXY"
  
  connection_type      = "INTERNET"
  integration_method   = "POST"
  integration_uri      = aws_lambda_function.api.invoke_arn
  payload_format_version = "2.0"
}

# Default route
resource "aws_apigatewayv2_route" "default" {
  api_id    = aws_apigatewayv2_api.main.id
  route_key = "$default"
  target    = "integrations/${aws_apigatewayv2_integration.lambda.id}"
}

# Stage
resource "aws_apigatewayv2_stage" "default" {
  api_id      = aws_apigatewayv2_api.main.id
  name        = "$default"
  auto_deploy = true

  default_route_settings {
    detailed_metrics_enabled = false  # Save costs
    throttling_burst_limit   = 100
    throttling_rate_limit    = 50
  }

  tags = {
    Name = "${var.project_name}-api-stage"
  }
}

# Lambda permission for API Gateway
resource "aws_lambda_permission" "api_gateway" {
  statement_id  = "AllowExecutionFromAPIGateway"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.api.function_name
  principal     = "apigateway.amazonaws.com"

  source_arn = "${aws_apigatewayv2_api.main.execution_arn}/*/*"
}
