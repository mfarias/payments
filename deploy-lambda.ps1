# Deploy Payments API as Lambda function
param(
    [string]$AwsRegion = "us-east-1",
    [string]$ProjectName = "payments-api"
)

Write-Host "🚀 Deploying $ProjectName as Lambda function to $AwsRegion" -ForegroundColor Green
Write-Host "Estimated cost: ~$4.20/month (with provisioned concurrency)" -ForegroundColor Yellow

# Check prerequisites
$tools = @("aws", "terraform", "dotnet")
foreach ($tool in $tools) {
    if (!(Get-Command $tool -ErrorAction SilentlyContinue)) {
        Write-Error "$tool is not installed or not in PATH"
        exit 1
    }
}

# Step 1: Build and package Lambda
Write-Host "`n📦 Building Lambda package..." -ForegroundColor Cyan
Set-Location "Payments\Payments.Api"

dotnet restore
dotnet publish -c Release -r linux-x64 --self-contained false -o publish

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}

# Create Lambda zip
Set-Location publish
if (Test-Path "..\..\..\terraform\payments-api.zip") {
    Remove-Item "..\..\..\terraform\payments-api.zip"
}
Compress-Archive -Path * -DestinationPath "..\..\..\terraform\payments-api.zip"
Write-Host "✅ Lambda package created" -ForegroundColor Green

# Step 2: Deploy infrastructure
Write-Host "`n🏗️ Deploying infrastructure..." -ForegroundColor Cyan
Set-Location "..\..\..\terraform"

terraform init
terraform plan -var="aws_region=$AwsRegion" -var="project_name=$ProjectName"

$confirm = Read-Host "`nProceed with deployment? (y/N)"
if ($confirm -ne 'y' -and $confirm -ne 'Y') {
    Write-Host "Deployment cancelled" -ForegroundColor Red
    exit 0
}

terraform apply -var="aws_region=$AwsRegion" -var="project_name=$ProjectName" -auto-approve

if ($LASTEXITCODE -ne 0) {
    Write-Error "Terraform deployment failed!"
    exit 1
}

# Step 3: Display results
Write-Host "`n🎉 Deployment successful!" -ForegroundColor Green

$apiUrl = terraform output -raw api_gateway_url
$lambdaName = terraform output -raw lambda_function_name

Write-Host "`n📋 Deployment Summary:" -ForegroundColor Cyan
Write-Host "API URL: $apiUrl" -ForegroundColor White
Write-Host "Lambda Function: $lambdaName" -ForegroundColor White

Write-Host "`n🧪 Test Commands:" -ForegroundColor Cyan
Write-Host "Health Check: curl $apiUrl/health" -ForegroundColor White
Write-Host "Create Payment: curl -X POST $apiUrl/api/payment -H 'Content-Type: application/json' -d '{`"amount`":100,`"paymentMethod`":1}'" -ForegroundColor White

Write-Host "`n💰 Monthly Cost: ~$4.20 (with 1 provisioned concurrency)" -ForegroundColor Yellow
Write-Host "Free tier eligible for 12 months!" -ForegroundColor Green

Set-Location ..
