-- You can both read and write in the virtual console.
-- Use cs.ShouldExecute() instead of infinite loops so that the host environment can terminate the loop.
while (cs.ShouldExecute()) do
    cs.Print("> ")
    x = cs.Readln()
    cs.Println(x)
end
