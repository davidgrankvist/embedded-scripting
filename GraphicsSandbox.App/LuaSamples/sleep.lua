-- Use cs.Sleep to sleep for a number of milliseconds.
for i = 1, 5 do
    cs.Println("hello " .. i)
    cs.Sleep(1000)
end

cs.Println("done")