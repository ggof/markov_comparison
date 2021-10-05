defmodule ExMarkov.Text do
  @chars String.graphemes(",.\/\\\"'?!:;[]{}\|()-_\n\t]")

  defp format(line) do
    line
    |> String.downcase()
    |> String.replace(@chars, " ")
    |> String.split()
    |> Enum.filter(&smaller_than_3/1)
  end

  defp smaller_than_3(word) do
    String.length(word) > 2
  end


  def parse_file(path, len) do
    File.stream!(path)
    |> Enum.flat_map(&format/1)
    |> Enum.chunk_every(len, 1)
    |> Enum.frequencies_by(fn x -> Enum.join(x, " ") end)
  end
end
