resource "aws_cognito_user_pool" "fastfood_user_pool" {
  name = "${var.projectName}-UserPool"

  # Configurações de política de senha
  password_policy {
    minimum_length                   = 8
    require_lowercase                = true
    require_uppercase                = true
    require_numbers                  = true
    require_symbols                  = true
    temporary_password_validity_days = 7
  }

  mfa_configuration = "OFF"

  account_recovery_setting {
    recovery_mechanism {
      name = "admin_only"
    }
  }

  lambda_config {
    post_confirmation              = ""
    pre_sign_up                    = ""
    pre_authentication             = ""
    define_auth_challenge          = ""
    create_auth_challenge          = ""
    verify_auth_challenge_response = ""
  }

  required_attributes = ["email", "name"]

  schema {
    name                = "cpf"
    attribute_data_type = "String"
    required            = true
    mutable             = false
  }

  resource "aws_cognito_user_pool_client" "fastfood_user_pool_client" {
    name                = "${var.projectName}-ClientUserPool"
    user_pool_id        = aws_cognito_user_pool.fastfood_user_pool.id
    generate_secret     = false
    explicit_auth_flows = ["ADMIN_NO_SRP_AUTH"]
  }
}