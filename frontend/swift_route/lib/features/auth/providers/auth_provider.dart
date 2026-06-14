import 'package:dio/dio.dart';
import 'package:flutter_riverpod/legacy.dart';
import 'package:swift_route/core/api/api_client.dart';
import 'package:swift_route/core/api/api_endpoints.dart';
import 'package:swift_route/core/models/auth_model.dart';

import '../../../core/api/api_provider.dart';

enum AuthStatus { initial, loading, authenticated, unauthenticated, error }

class AuthState {
  final AuthStatus status;
  final AuthModel? data;
  final String? errorMessage;

  AuthState({
    required this.status,
    this.data,
    this.errorMessage
  });

  AuthState copyWith({
    AuthStatus? status,
    AuthModel? data,
    String? errorMessage,
  }) {
    return AuthState(
      status: status ?? this.status,
      data: data ?? this.data,
      errorMessage: errorMessage ?? this.errorMessage,
    );
  }
}

class AuthNotifier extends StateNotifier<AuthState> {
  final ApiClient _apiClient;

  AuthNotifier(this._apiClient) : super(AuthState(status: AuthStatus.initial));

  Future<void> login(String email, String password) async {
    state = state.copyWith(status: AuthStatus.loading);

    try {
      final response = await _apiClient.dio.post(
        ApiEndpoints.login,
        data: {
          'email': email,
          'password': password
        },
      );

      final loginResponse = AuthModel.fromJson(response.data);
      _apiClient.setTokens(loginResponse.accessToken, loginResponse.refreshToken);

      state = state.copyWith(
        status: AuthStatus.authenticated,
        data: loginResponse,
        errorMessage: null,
      );
    } on DioException catch (e) {
      final message = e.response?.statusCode == 401
          ? 'Invalid email or password'
          : 'Something went wrong, please try again';

      state = state.copyWith(
        status: AuthStatus.error,
        errorMessage: message
      );
    }
  }

  Future<void> logout() async {
    try {
      await _apiClient.dio.post(ApiEndpoints.logout);
    } finally {
      _apiClient.clearTokens();
      state = AuthState(status: AuthStatus.unauthenticated);
    }
  }
}

final authProvider = StateNotifierProvider<AuthNotifier, AuthState>((ref) {
  final apiClient = ref.watch(apiClientProvider);
  return AuthNotifier(apiClient);
});