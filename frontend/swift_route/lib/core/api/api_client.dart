import 'package:dio/dio.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

class ApiClient {
  static String get baseUrl => dotenv.env['API_URL'] ?? 'http://localhost:5243';

  late final Dio dio;
  late final Dio _refreshDio;

  String? _accessToken;
  String? _refreshToken;
  bool _isRefreshing = false;

  ApiClient() {
    dio = Dio(BaseOptions(
      baseUrl: baseUrl,
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
      headers: {'Content-Type': 'application/json'},
    ));

    _refreshDio = Dio(BaseOptions(
      baseUrl: baseUrl,
      headers: {'Content-Type': 'application/json'},
    ));

    dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: _onRequest,
        onError: _onError
      ),
    );
  }

  void setTokens(String accessToken, String refreshToken) {
    _accessToken = accessToken;
    _refreshToken = refreshToken;
  }

  void clearTokens() {
    _accessToken = null;
    _refreshToken = null;
  }

  void _onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    if (_accessToken != null) {
      options.headers['Authorization'] = 'Bearer $_accessToken';
    }
    handler.next(options);
  }

  Future<void> _onError(
      DioException err,
      ErrorInterceptorHandler handler,
      ) async {
    if (err.response?.statusCode != 401 || _refreshToken == null || _isRefreshing) {
      handler.next(err);
      return;
    }

    _isRefreshing = true;

    try {
      final response = await _refreshDio.post(
        '/auth/refresh',
        data: {'refreshToken': _refreshToken},
      );

      _accessToken = response.data['accessToken'];
      _refreshToken = response.data['refreshToken'];

      err.requestOptions.headers['Authorization'] = 'Bearer $_accessToken';
      final retryResponse = await dio.fetch(err.requestOptions);
      handler.resolve(retryResponse);
    } catch (e) {
      clearTokens();
      handler.next(err);
    } finally {
      _isRefreshing = false;
    }
  }
}