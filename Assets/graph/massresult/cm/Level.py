#!/usr/bin/python
from PIL import Image
import os
import numpy as np

level = [3,5,1,7,2,8,1,8,8,4,7,1]


ext = ".png"


x = 400
y = 400 * len(level)

levData = np.zeros([x,y,3],dtype=np.uint8)
levData.fill(255)
levImg = Image.fromarray(levData)
levImg.save("level.png")


x0 = 0
y0 = 0
x1 = 400
y1 = 400

first = "" + str(level[0]) + str(level[0]) + ext
firImg = Image.open(first)
levImg.paste(firImg, (x0,y0,x1,y1))

for i in range(0,len(level)-1):
  fn = "" + str(level[i]) + str(level[i+1]) + ext
  img = Image.open(fn)

  x0=x1
  x1+=400
  
  levImg.paste(img, (x0,y0,x1,y1))
  

levImg.save("level.png")
  
  

