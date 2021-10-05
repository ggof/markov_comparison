import 'dart:io';
import 'dart:isolate';

import 'author_parser.dart';

final texts = '../Texts/';

void parseAuthor(String path) => AuthorParser(path).parse();
Future<void> main(List<String> arguments) async {
  final start = DateTime.now().millisecondsSinceEpoch;
  var count = 0;
  final port = ReceivePort();

  await for (final dir in Directory(texts).list()) {
    count++;
    Isolate.spawn(parseAuthor, dir.path, onExit: port.sendPort);
  }

  await port.forEach((_) {
    if (--count == 0) port.close();
  });

  final elapsed = DateTime.now().millisecondsSinceEpoch - start;
  stdout.writeln('Done, took $elapsed ms');
}
