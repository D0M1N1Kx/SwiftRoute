class ApiEndpoints {
  static const String login = '/auth/login';
  static const String logout = '/auth/logout';
  static const String refresh = '/auth/refresh';

  static const String users = '/users';

  static String userById(String id) => '/users/$id';

  static const String orders = '/orders';

  static String orderById(String id) => '/orders/$id';

  static String assignCourier(String id) => '/orders/$id/assign';

  static String updateOrderStatus(String id) => '/orders/$id/status';

  static const String warehouses = '/warehouse';

  static String warehouseById(String id) => '/warehouses/$id';

  static String warehouseInventory(String id) => '/warehouses/$id/inventory';

  static const String courierLocations = '/couriers/locations';
  static const String updateLocation = '/couriers/location';

  static const String workerCountsByRole = '/status/workers';
}
