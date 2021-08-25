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
    punc = ",./\\\"'?!:;[]{}|()-_\n\t"
    dir = "../Texts/"
    printer = "Auteur \"%s\": \"%s\" avec %d repetitions\n"
)

func format(s string) string {
	for _, p := range punc {
		s = strings.ReplaceAll(s, string(p), " ")
	}
	return strings.ToLower(s)
}

func check(e error) {
	if e != nil {
		panic(e)
	}
}

func main() {
	start := time.Now()

	authors, ngram := readDir()

    wg := &sync.WaitGroup{}
    wg.Add(len(authors))

	for _, dir := range authors {
		go Parse(dir, ngram, wg)
	}

    wg.Wait()

	fmt.Println("Done parsing and sorting. took ", time.Since(start).String())
}


func Parse(author string, ngram int, wg *sync.WaitGroup) {
	li := NewListe(author)

    path := dir + author

	dir, err := os.Open(path)
	check(err)
	defer dir.Close()

	filenames, err := dir.Readdir(0)
	check(err)

	for _, file := range filenames {
		filepath := path + "/" + file.Name()
		parseFile(filepath, ngram, li)
	}

    max := li.Max()

    fmt.Printf(printer,li.Author, max.Word, max.Count )

    wg.Done()
}

func parseFile(filepath string, ngramlength int, li *Liste) {
	var ngram []string

	file, err := os.Open(filepath)
	check(err)
	defer file.Close()

	scanner := bufio.NewScanner(file)

	for scanner.Scan() {
		line := scanner.Text()
		line = format(line)
		for _, word := range strings.Split(line, " ") {
			if len(word) < 3 {
				continue
			}

			ngram = append(ngram, word)

			if len(ngram) > ngramlength-1 {
				li.add(strings.Join(ngram, " "))
				ngram = ngram[1:]
			}
		}
	}
}

func readDir() ([]string, int) {
	openedDir, err := os.Open(dir)
	check(err)
	defer openedDir.Close()

	dirs, err := openedDir.Readdirnames(0)
	check(err)

	return dirs, len(dirs)
}
