output "api_gateway_url" {
  description = "URL of the API Gateway"
  value       = aws_apigatewayv2_api.main.api_endpoint
}

output "lambda_function_name" {
  description = "Name of the Lambda function"
  value       = aws_lambda_function.api.function_name
}

output "rds_endpoint" {
  description = "RDS instance endpoint"
  value       = aws_db_instance.main.endpoint
  sensitive   = true
}

output "database_name" {
  description = "Database name"
  value       = aws_db_instance.main.db_name
}

output "estimated_monthly_cost" {
  description = "Estimated monthly cost for testing"
  value = {
    lambda_requests = "Free (up to 1M requests)"
    lambda_provisioned_concurrency = var.enable_provisioned_concurrency ? "$4.20" : "$0"
    api_gateway = "Free (up to 1M requests)"
    rds_t3_micro = "Free (750 hours/month)"
    cloudwatch_logs = "Free (5GB/month)"
    total_estimated = var.enable_provisioned_concurrency ? "~$4.20/month" : "~$0.20/month"
  }
}
