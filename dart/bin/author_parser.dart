import 'dart:io';

class AuthorParser {
  final String _dir;
  final _ngrams = <String, int>{};
  static const badChars = r"""[,.\\/\"'?!:;\[\]\{\}|\(\)-_\n\t]+""";

  AuthorParser(this._dir);

  Future<void> parse() async {
    final author = _dir.split('/').last;
    final files = Directory(_dir).list();

    final futures = <Future<void>>[];

    await files.forEach((file) => futures.add(parseFile(file.path)));

    await Future.wait(futures);

    var key = "";
    var val = 0;

    _ngrams.forEach((k, v) {
      if (v > val) {
        key = k;
        val = v;
      }
    });

    stdout.writeln('Author $author: "$key" with $val repetitions.');
  }

  Future<void> parseFile(String path) async {
    final lines = await File(path).readAsLines();
    final ngram = <String>[];

    for (final line in lines.map(toCleanString)) {
      for (final word in line.split(' ')) {
        if (word.length < 3) {
          continue;
        }

        ngram.add(word);
        if (ngram.length != 3) {
          continue;
        }

        _ngrams.update(ngram.join(' '), (value) => value + 1,
            ifAbsent: () => 1);

        ngram.remove(ngram.first);
      }
    }
  }

  String toCleanString(String input) =>
      input.toLowerCase().replaceAll(RegExp(badChars), ' ');
}
