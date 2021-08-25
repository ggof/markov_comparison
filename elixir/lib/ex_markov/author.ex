defmodule ExMarkov.Author do
    defp to_task(path, text, len) do
        Task.async(fn -> ExMarkov.Text.parse_file("#{path}/#{text}", len) end)
    end

    defp value({_k, v}) do
      v
    end

    def parse_author(path, len) do
        {ngram, reps} = File.ls!(path)
        |> Enum.map(fn x -> to_task(path, x, len) end)
        |> Task.await_many()
        |> Enum.reduce(Map.new(), fn x, acc -> Map.merge(x, acc, fn _, a, b -> a + b end) end)
        |> Enum.max_by(&value/1)

        author = path
        |> String.split("/")
        |> List.last()

        IO.puts "Auteur \"#{author}\": \"#{ngram}\" avec #{reps} repetitions"
    end
end
