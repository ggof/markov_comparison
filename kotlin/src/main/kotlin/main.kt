import java.io.File
import java.util.*
import kotlinx.coroutines.*

fun toBigWordsWithoutDelimiters(line: String) =
        line
                .lowercase(Locale.getDefault())
                .filter { !",./\\\"'?!:;[]{}|()-_\n\t".contains(it) }
                .split(" ")
                .filter { it.length > 2 }

fun readFile(file: String) =
        File(file)
                .bufferedReader()
                .lineSequence()
                .flatMap { toBigWordsWithoutDelimiters(it) }
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

suspend fun Sequence<Job>.joinAll(): Unit = forEach { it.join() }

fun main(args: Array<String>) = runBlocking {
    val now = System.currentTimeMillis()
    val dir = "../Texts"

    File(dir)
            .walk()
            .filter { it.isDirectory && it.name != "Texts"}
            .map { launch { parseAuthor(it.path) } }
            .joinAll()

    println("Done. took ${System.currentTimeMillis() - now} ms.")
}
