variable "aws_region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "project_name" {
  description = "Name of the project"
  type        = string
  default     = "payments-api"
}

variable "environment" {
  description = "Environment name"
  type        = string
  default     = "test"
}

variable "db_name" {
  description = "Database name"
  type        = string
  default     = "PaymentsDb"
}

variable "db_username" {
  description = "Database master username"
  type        = string
  default     = "postgres"
}

variable "lambda_memory" {
  description = "Lambda memory in MB"
  type        = number
  default     = 512
}

variable "lambda_timeout" {
  description = "Lambda timeout in seconds"
  type        = number
  default     = 30
}

variable "enable_provisioned_concurrency" {
  description = "Enable provisioned concurrency to avoid cold starts"
  type        = bool
  default     = true
}
