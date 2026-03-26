sed -i 's/p.MapRegionX = (p.AbsX >> 3) - 6;/p.MapRegionX = (p.AbsX >> 3);/g' AeroScape.Server.Network/Frames/LoginFrames.cs
sed -i 's/p.MapRegionY = (p.AbsY >> 3) - 6;/p.MapRegionY = (p.AbsY >> 3);/g' AeroScape.Server.Network/Frames/LoginFrames.cs
sed -i 's/p.CurrentX = p.AbsX - 8 \* p.MapRegionX;/p.CurrentX = p.AbsX - 8 \* (p.MapRegionX - 6);/g' AeroScape.Server.Network/Frames/LoginFrames.cs
sed -i 's/p.CurrentY = p.AbsY - 8 \* p.MapRegionY;/p.CurrentY = p.AbsY - 8 \* (p.MapRegionY - 6);/g' AeroScape.Server.Network/Frames/LoginFrames.cs
