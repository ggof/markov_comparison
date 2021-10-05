package main

import (
	"bufio"
	"fmt"
	"os"
	"strings"
	"sync"
	"time"
)

const (
	punc        = ",./\\\"'?!:;[]{}|()-_\n\t"
	dir         = "../Texts/"
	fmtResult   = "Auteur \"%s\": \"%s\" avec %d repetitions\n"
	sizeOfNGram = 3
)

func main() {
	start := time.Now()

	authors := readDir()

	wg := sync.WaitGroup{}
	wg.Add(len(authors))

	for _, author := range authors {
		go Parse(author, sizeOfNGram, wg.Done)
	}

	wg.Wait()
	fmt.Println("Done parsing and sorting. took ", time.Since(start).String())
}

func readDir() []string {
	dirs, err := os.ReadDir(dir)
	check(err)

	names := make([]string, len(dirs))
	for i, dir := range dirs {
		names[i] = dir.Name()
	}
	return names
}

func check(e error) {
	if e != nil {
		panic(e)
	}
}

func Parse(author string, size int, done func()) {
	path := dir + author

	files, err := os.ReadDir(path)
	check(err)

	acc := make(map[string]int)

	for _, file := range files {
		filepath := path + "/" + file.Name()
		parseFile(filepath, size, &acc)
	}

	key, val := max(acc)
	fmt.Printf(fmtResult, author, key, val)

	done()
}

func parseFile(path string, ngramlength int, acc *map[string]int) {
	ngram := make([]string, 0, ngramlength)

	file, err := os.Open(path)
	defer file.Close()
	check(err)

	scanner := bufio.NewScanner(file)

	for scanner.Scan() {
		for _, word := range formattedWords(scanner.Text()) {
			if isTooShort(word) {
				continue
			}

			ngram = append(ngram, word)

			if len(ngram) == ngramlength {
				(*acc)[strings.Join(ngram, " ")] += 1
				ngram = ngram[1:]
			}
		}
	}
}

func formattedWords(line string) []string {
	return strings.Split(format(line), " ")
}

func isTooShort(word string) bool {
	return len(word) < 3
}

func format(s string) string {
	for _, p := range punc {
		s = strings.ReplaceAll(s, string(p), " ")
	}
	return strings.ToLower(s)
}

func max(m map[string]int) (max string, count int) {
	for key, value := range m {
		if value > count {
			max, count = key, value
		}
	}
	return
}
