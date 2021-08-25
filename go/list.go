package main

type Word struct {
	Word string
	Count int64
}

type Liste struct {
	Author string
	Words map[string]Word
}

func NewListe(auteur string) *Liste {
	return &Liste{Author: auteur, Words: make(map[string]Word)}
}

func (l *Liste) add(mot string) {
	if w, ok := l.Words[mot]; ok {
		w.Count += 1
		l.Words[mot] = w
	} else {
		l.Words[mot] = Word{Word: mot, Count: 1}
	}
}

func (l *Liste) Max() (max Word) {
	for _, value := range l.Words {
        if value.Count > max.Count {
            max = value
        }
	}
    
    return
}
