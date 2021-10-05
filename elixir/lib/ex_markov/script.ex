defmodule ExMarkov.Script do
  alias ExMarkov.Author

  @dir "../Texts/"

  defp to_task(author) do
    Task.async(fn -> Author.parse_author(@dir <> author, 3) end)
  end

  def main(_args \\ []) do
    start = :os.system_time(:millisecond)

    File.ls!(@dir)
    |> Enum.map(&to_task/1)
    |> Task.await_many()

    diff = :os.system_time(:millisecond) - start
    IO.puts("Done parsing all authors. took #{diff}ms.")
  end
end
