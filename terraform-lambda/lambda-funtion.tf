resource "aws_lambda_function" "lambda_function" {
  function_name = "AutheticatorClient-${var.projectName}"
  role          = var.labRole
  handler       = "AuthLambda::AuthLambda.Function::FunctionHandler"
  runtime       = "dotnet8"
  architecture  = "x86_64"

  filename         = "${path.module}/function.zip"
  source_code_hash = filebase64sha256("${path.module}/function.zip")
}