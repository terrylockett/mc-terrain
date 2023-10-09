# McTerrain 

Terrain for Unity using the Marching Cubes algorithm. 

## Installation

- **Unity Package Manager(UPM)**: Add the following to *Packages/manifest.json*:
   - `"ca.terrylockett.mc-terrain": "https://github.com/terrylockett/mc-terrain.git#v1.0.1"`

## Usage
### Setup
- Drag `Packages/McTerrain/Prefab/McTerrainPrefab` into your scene then rename It.
- Select the terrain and press `Generte Mesh` in the inspector to create a mesh.
- Use the Custom Editor in the inspector to modify the Terrain.

### Customising Terrain
- With the Terrain selected you can customise it via the custom editor in the Inspector.
- This is not fully complete so good luck :)

The editor has 3 sections that can be toggles on via check-boxes: `Terrain Size`, `Perlin Settings` and `Sculpt Mode`

#### Terrain Size
- `Chunks` this is number of chunks used in **both** x and z directions. So value of 4 means 16 chunks or a value of 5 means 25 chunks... not misleading at all :)
- `Map Height` the number of cubes in the Y direction. This can cause a performance hit if you crank it.

#### Perlin Settings
These settings configure the "rolling hills" that the terrain is generated with.
- `Amplitude` The size of the hills in the Y dir. If you want fat terrain set this to 0 (it will change it 0.001 which is minimum).
- `X Scalar` (terrible name.. will fix someday) This is the frequency of hills in the X direction.
- `Z Scalar` This is the frequency of hills in the Z direction.
- `X Offset` Change this for different random generation in the X dir. 
- `Z Offset` Change for different random generation in the Z dir.
- `Iso Level` Should be renamed to surface level soon. This is the height at which terrain will become visible. you will have to modify this slider to something that looks good after any changes any of the amplitude values.

#### Sculpt Mode
Enable this to sculpt the terrain using some very basic and janky sculpt tools.
when enabled `left click` on terrain in editor window to add volume and `shift + left click` to subtract volume.

Config options:
- `Brush Size` The size of the volume being added to the terrain.
- `Brush Strength` This is how quickly terrain is added when holding left click. the scaling on this is fucked so i recommend keeping the value below 0.1. The performance may be hardware dependent :shrug: so you may need to do some denial and error.

### Runtime Deformation
- At the moment its pretty basic. On The MarchingCubesTerrain object there is `makeBombHole(Vector3 point, float radius)` which can be used to modify the terrain during runtime.
