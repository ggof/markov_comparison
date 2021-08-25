import java.io.File
import java.util.Locale
import kotlin.system.measureTimeMillis
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.joinAll
import kotlinx.coroutines.launch
import kotlinx.coroutines.runBlocking

const val bannedChars = ",./\\\"'?!:;[]{}|()-_\n\t"

fun toBigWordsWithoutDelimiters(line: String): List<String> =
	line
		.lowercase(Locale.getDefault())
		.filterNot { bannedChars.contains(it) }
		.split(" ")
		.filter { it.length > 2 }

fun readFile(file: String) =
	File(file)
		.bufferedReader()
		.lineSequence()
		.flatMap { line -> toBigWordsWithoutDelimiters(line) }
		.windowed(3)

fun parseAuthor(dir: String) {
	val fav =
		File(dir)
			.walk()
			.filter { it.isFile }
			.map { readFile(it.path) }
			.fold(mutableMapOf<String, Int>()) { map, list ->
				list.forEach {
					val key = it.joinToString(" ")
					val value = map.getOrDefault(key, 0)
					map[key] = value + 1
				}
				map
			}
			.maxByOrNull { it.value }

	if (fav != null) {
		val author = dir.split("/").last()
		println("Auteur \"$author\": \"${fav.key}\" avec ${fav.value} repetitions")
	} else {
		println("pas de ngram (oups?)")
	}
}

fun main() = runBlocking {
	val textsDirectory = "../Texts"

	val timeToExecute = measureTimeMillis {
		File(textsDirectory)
			.walk()
			.filter { it.isDirectory && it.name != "Texts" }
			.map {
				launch(Dispatchers.Default) {
					parseAuthor(it.path)
				}
			}
			.toList()
			.joinAll()
	}

	println("Done. took $timeToExecute ms.")
}
