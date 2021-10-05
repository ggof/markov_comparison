defmodule ExMarkov.Author do
  alias ExMarkov.Text

  defp to_task(path, text, len) do
    Task.async(fn -> Text.parse_file("#{path}/#{text}", len) end)
  end

  defp print({ngram, reps}, author) do
    IO.puts("Auteur \"#{author}\": \"#{ngram}\" avec #{reps} repetitions")
  end

  defp reducer(x, acc) do
    Map.merge(x, acc, fn _, v1, v2 -> v1 + v2 end)
  end

  defp value({_, v}) do
    v
  end

  def parse_author(path, len) do
    author =
      path
      |> String.split("/")
      |> List.last()

    File.ls!(path)
    |> Enum.map(fn x -> to_task(path, x, len) end)
    |> Task.await_many()
    |> Enum.reduce(Map.new(), &reducer/2)
    |> Enum.max_by(&value/1)
    |> print(author)
  end
end
