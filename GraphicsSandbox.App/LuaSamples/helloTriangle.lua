-- Use the draw functions to paint to the canvas.
-- You can draw multiple shapes between the begin/end draw.
-- If you do a new begin/end draw, the canvas is automatically cleared.
firstTriangle = { 
    a = { x = 0, y = 0 },
    b = { x = 100, y = 0 },
    c = { x = 0, y = 100 },
    color = { r = 255, g = 0, b = 0, a = 255 },
}

secondTriangle = { 
    a = { x = 100, y = 0 },
    b = { x = 100, y = 100 },
    c = { x = 0, y = 100 },
    color = { r = 0, g = 0, b = 255, a = 255 },
}

cs.BeginDraw()
cs.DrawTriangle(firstTriangle.a, firstTriangle.b, firstTriangle.c, firstTriangle.color)
cs.DrawTriangle(secondTriangle.a, secondTriangle.b, secondTriangle.c, secondTriangle.color)
cs.EndDraw()
