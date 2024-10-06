resource "aws_api_gateway_rest_api" "my_api" {
  name        = "AuthClient-${var.projectName}"
  description = "API Gateway for Auth"
}

resource "aws_api_gateway_resource" "auth_resource" {
  rest_api_id = aws_api_gateway_rest_api.my_api.id
  parent_id   = aws_api_gateway_rest_api.my_api.root_resource_id
  path_part   = "auth"
}
