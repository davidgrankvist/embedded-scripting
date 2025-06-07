-- Here's a simple render loop that moves a triangle back and forth.

triangle = {
    a = { x = 0, y = 0 },
    b = { x = 100, y = 0 },
    c = { x = 0, y = 100 },
    color = { r = 255, g = 0, b = 0, a = 255 },
}

xmax = 200
xmin = 0
sign = 1
speed = 5

while (cs.ShouldExecute()) do

    offset = speed * sign
    triangle.a.x = triangle.a.x + offset
    triangle.b.x = triangle.b.x + offset
    triangle.c.x = triangle.c.x + offset

    if (triangle.a.x < 0) then
        sign = 1
    elseif (triangle.a.x > xmax) then
        sign = -1
    end


    cs.BeginDraw()
    cs.DrawTriangle(triangle.a, triangle.b, triangle.c, triangle.color)
    cs.EndDraw()

    -- super handwaved, but around 60 FPS if the rest is really quick
    cs.Sleep(16)
end