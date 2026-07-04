import 'package:dio/dio.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class ApiClient {
  static String get baseUrl => dotenv.env['API_URL'] ?? 'http://localhost:5243';

  late final Dio dio;
  late final Dio _refreshDio;

  final _storage = const FlutterSecureStorage();
  bool _isRefreshing = false;

  ApiClient() {
    dio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        connectTimeout: const Duration(seconds: 10),
        receiveTimeout: const Duration(seconds: 10),
        headers: {'Content-Type': 'application/json'},
      ),
    );

    _refreshDio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        headers: {'Content-Type': 'application/json'},
      ),
    );

    dio.interceptors.add(
      InterceptorsWrapper(onRequest: _onRequest, onError: _onError),
    );
  }

  Future<void> setTokens(String accessToken, String refreshToken) async {
    await _storage.write(key: 'access_token', value: accessToken);
    await _storage.write(key: 'refresh_token', value: refreshToken);
  }

  Future<void> clearTokens() async {
    await _storage.delete(key: 'access_token');
    await _storage.delete(key: 'refresh_token');
  }

  Future<void> _onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    final accessToken = await _storage.read(key: 'access_token');
    if (accessToken != null) {
      options.headers['Authorization'] = 'Bearer $accessToken';
    }
    handler.next(options);
  }

  Future<void> _onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    final refreshToken = await _storage.read(key: 'refresh_token');

    if (err.response?.statusCode != 401 ||
        refreshToken == null ||
        _isRefreshing) {
      handler.next(err);
      return;
    }

    _isRefreshing = true;

    try {
      _refreshDio.options.headers.remove('Authorization');

      final response = await _refreshDio.post(
        '/auth/refresh',
        data: {'refreshToken': refreshToken},
      );

      final newAccessToken = response.data['accessToken'];
      final newRefreshToken = response.data['refreshToken'];

      await setTokens(newAccessToken, newRefreshToken);

      final requestOptions = err.requestOptions;

      requestOptions.headers['Authorization'] = 'Bearer $newAccessToken';

      final retryResponse = await dio.fetch(requestOptions);

      handler.resolve(retryResponse);
    } catch (e) {
      await clearTokens();
      handler.next(err);
    } finally {
      _isRefreshing = false;
    }
  }
}
