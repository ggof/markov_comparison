defmodule ExMarkov.Text do

    @chars String.graphemes(",.\/\\\"'?!:;[]{}\|()-_\n\t]")

    defp remove_chars (line) do
        String.replace(line, @chars, " ")
    end

    defp smaller_than_3 (word) do
        String.length(word) > 2
    end

    def parse_file(path, len) do
        File.stream!(path)
        |> Enum.map(&String.downcase/1)
        |> Enum.map(&remove_chars/1)
        |> Enum.flat_map(&String.split/1)
        |> Enum.filter(&smaller_than_3/1)
        |> Enum.chunk_every(len, 1)
        |> Enum.frequencies_by(fn x -> Enum.join(x, " ") end)
    end

end
