class OrderModel {
  final String id;
  final String trackingNumber;
  final String recipientName;
  final String recipientPhone;
  final String pickupAddress;
  final double pickupLat;
  final double pickupLng;
  final String deliveryAddress;
  final double deliveryLat;
  final double deliveryLng;
  final String status;
  final String? notes;
  final String dispatcherName;
  final String? courierName;
  final DateTime createdAt;
  final DateTime? deliveredAt;

  OrderModel({
    required this.id,
    required this.trackingNumber,
    required this.recipientName,
    required this.recipientPhone,
    required this.pickupAddress,
    required this.pickupLat,
    required this.pickupLng,
    required this.deliveryAddress,
    required this.deliveryLat,
    required this.deliveryLng,
    required this.status,
    this.notes,
    required this.dispatcherName,
    this.courierName,
    required this.createdAt,
    this.deliveredAt,
  });

  factory OrderModel.fromJson(Map<String, dynamic> json) {
    return OrderModel(
      id: json['id'],
      trackingNumber: json['trackingNumber'],
      recipientName: json['recipientName'],
      recipientPhone: json['recipientPhone'],
      pickupAddress: json['pickupAddress'],
      pickupLat: json['pickupLat'].toDouble(),
      pickupLng: json['pickupLng'].toDouble(),
      deliveryAddress: json['deliveryAddress'],
      deliveryLat: json['deliveryLat'].toDouble(),
      deliveryLng: json['deliveryLng'].toDouble(),
      status: json['status'],
      notes: json['notes'],
      dispatcherName: json['dispatcherName'],
      courierName: json['courierName'],
      createdAt: DateTime.parse(json['createdAt']),
      deliveredAt: json['deliveredAt'] != null
          ? DateTime.parse(json['deliveredAt'])
          : null,
    );
  }
}