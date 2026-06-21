class CourierLocationModel {
  final String courierId;
  final String courierName;
  final double latitude;
  final double longitude;
  final double? speedKmh;
  final double? heading;
  final DateTime recordedAt;

  CourierLocationModel({
    required this.courierId,
    required this.courierName,
    required this.latitude,
    required this.longitude,
    this.speedKmh,
    this.heading,
    required this.recordedAt,
  });

  factory CourierLocationModel.fromJson(Map<String, dynamic> json) {
    return CourierLocationModel(
      courierId: json['courierId'],
      courierName: json['courierName'],
      latitude: json['latitude'].toDouble(),
      longitude: json['longitude'].toDouble(),
      speedKmh: json['speedKmh']?.toDouble(),
      heading: json['heading']?.toDouble(),
      recordedAt: DateTime.parse(json['recordedAt']),
    );
  }
}
