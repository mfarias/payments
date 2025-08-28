# RDS Subnet Group (using public subnets for free tier)
resource "aws_db_subnet_group" "main" {
  name       = "${var.project_name}-db-subnet-group"
  subnet_ids = aws_subnet.public[*].id

  tags = {
    Name = "${var.project_name}-db-subnet-group"
  }
}

# RDS Security Group
resource "aws_security_group" "rds" {
  name_prefix = "${var.project_name}-rds-"
  vpc_id      = aws_vpc.main.id

  # Allow access from anywhere (for testing only)
  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
    description = "PostgreSQL access (testing only)"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "${var.project_name}-rds-sg"
  }
}

# Random password for RDS
resource "random_password" "db_password" {
  length      = 16
  special     = true
  # Exclude characters that RDS doesn't allow: /, @, ", and space
  override_special = "!#$%&*()-_=+[]{}<>:?"
}

# RDS Instance - Free tier optimized
resource "aws_db_instance" "main" {
  identifier     = "${var.project_name}-db"
  engine         = "postgres"
  engine_version = "16.3"
  instance_class = "db.t3.micro"  # Free tier eligible
  
  allocated_storage     = 20  # Free tier: 20GB
  max_allocated_storage = 20  # Prevent auto-scaling costs
  storage_type          = "gp2"
  storage_encrypted     = false  # Disable for free tier

  db_name  = var.db_name
  username = var.db_username
  password = random_password.db_password.result

  vpc_security_group_ids = [aws_security_group.rds.id]
  db_subnet_group_name   = aws_db_subnet_group.main.name

  # Minimal settings for cost savings
  backup_retention_period = 1
  backup_window          = "03:00-04:00"
  maintenance_window     = "sun:04:00-sun:05:00"

  skip_final_snapshot = true
  deletion_protection = false

  # Disable monitoring for cost savings
  monitoring_interval = 0
  
  # Public access for testing (Lambda can reach it)
  publicly_accessible = true

  tags = {
    Name = "${var.project_name}-database"
  }
}
