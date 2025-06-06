# LWGUI (Light Weight Shader GUI)

[中文](https://github.com/JasonMa0012/LWGUI/blob/dev/README_CN.md) | [English](https://github.com/JasonMa0012/LWGUI)

[![](https://dcbadge.vercel.app/api/server/WwBYGXqPEh)](https://discord.gg/WwBYGXqPEh)

A Lightweight, Flexible, Powerful **Unity Shader GUI** system.

Having been validated through numerous large-scale commercial projects, employing a succinct Material Property Drawer syntax allows for the realization of powerful Shader GUIs, substantially reducing development time, fostering ease of use and extensibility, and elevating the user experience for artists effectively.

![809c4a1c-ce80-48b1-b415-7e8d4bea716e](assets~/809c4a1c-ce80-48b1-b415-7e8d4bea716e-16616214059841.png)

![LWGUI](assets~/LWGUI.png)



| ![image-20240716183800118](./assets~/image-20240716183800118.png) | ![image-20240716184045776](./assets~/image-20240716184045776.png) |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| A more powerful Gradient editor than UE, with support for both Shader and C# | Insert images directly into the ShaderGUI to support the display of complex documents without having to jump to the browser |
| ![image-20250314160119094](./assets~/image-20250314160119094.png) |                                                              |
| **NEW: When recording material parameter animations in Timeline, automatically capture changes to Toggle's Keywords to enable switching material Keywords at runtime.** |                                                              |
| ![image-20220926025611208](./assets~/image-20220926025611208.png) | ![image-20230821205439889](./assets~/image-20230821205439889.png) |
| The search bar can also filter for properties that have been modified | Right-click to paste the attribute value by type             |




| With your sponsorship, I will update more actively. | 有你的赞助我会更加积极地更新                                 |
| --------------------------------------------------- | ------------------------------------------------------------ |
| [paypal.me/JasonMa0012](paypal.me/JasonMa0012)      | ![723ddce6-fb86-48ff-9683-a12cf6cff7a0](./assets~/723ddce6-fb86-48ff-9683-a12cf6cff7a0.jpg) |


<!--ts-->
* [LWGUI (Light Weight Shader GUI)](#lwgui-light-weight-shader-gui)
   * [Installation](#installation)
   * [Getting Started](#getting-started)
   * [Basic Drawers](#basic-drawers)
      * [Main &amp; Sub](#main--sub)
   * [Extra Drawers](#extra-drawers)
      * [Numeric](#numeric)
         * [SubToggle](#subtoggle)
         * [SubPowerSlider](#subpowerslider)
         * [SubIntRange](#subintrange)
         * [MinMaxSlider](#minmaxslider)
         * [KWEnum](#kwenum)
         * [SubEnum &amp; SubKeywordEnum](#subenum--subkeywordenum)
         * [Preset](#preset)
            * [Create Preset File](#create-preset-file)
            * [Edit Preset](#edit-preset)
         * [BitMask](#bitmask)
      * [Texture](#texture)
         * [Tex](#tex)
         * [Ramp](#ramp)
            * [ShaderLab](#shaderlab)
            * [C#](#c)
            * [Gradient Editor](#gradient-editor)
         * [Image](#image)
      * [Vector](#vector)
         * [Color](#color)
         * [Channel](#channel)
      * [Other](#other)
         * [Button](#button)
   * [Extra Decorators](#extra-decorators)
      * [Appearance](#appearance)
         * [Title &amp; SubTitle](#title--subtitle)
         * [Tooltip &amp; Helpbox](#tooltip--helpbox)
         * [ReadOnly](#readonly)
      * [Logic](#logic)
         * [PassSwitch](#passswitch)
      * [Structure](#structure)
         * [Advanced &amp; AdvancedHeaderProperty](#advanced--advancedheaderproperty)
      * [Condition Display](#condition-display)
         * [Hidden](#hidden)
         * [ShowIf](#showif)
   * [LWGUI Timeline Tracks](#lwgui-timeline-tracks)
      * [MaterialKeywordToggleTrack](#materialkeywordtoggletrack)
   * [Unity Builtin Drawers](#unity-builtin-drawers)
      * [Space](#space)
      * [Header](#header)
      * [Enum](#enum)
      * [IntRange](#intrange)
      * [KeywordEnum](#keywordenum)
      * [PowerSlider](#powerslider)
      * [Toggle](#toggle)
   * [Custom Shader GUI](#custom-shader-gui)
      * [Custom Header and Footer](#custom-header-and-footer)
      * [Custom Drawer](#custom-drawer)
   * [Contribution](#contribution)
<!--te-->

## Installation

1. Make sure your environment is compatible with LWGUI

   - LWGUI <1.17: **Unity 2017.4+**

   - LWGUI >=1.17: **Unity 2021.3+**
     - **Recommended minimum version: Unity 2022.2+, lower versions can be used but may have bugs**
   
2. Open your project
3. `Window > Package Manager > Add > Add package from git URL` , enter: `https://github.com/JasonMa0012/LWGUI.git`

   - You can also choose to manually download the Zip from Github，then: `Package Manager > Add package from disk`
   - For Unity 2017, please extract the Zip directly to the Assets directory



## Getting Started

1. Create a newer or use the existing Shader
2. Open the Shader in the code editor
3. At the bottom of the Shader, before the last large bracket, add line:`CustomEditor "LWGUI.LWGUI"`
4. Completed! Start using the following powerful Drawer to easily draw your Shader GUI
   - MaterialPropertyDrawer is C#-like attribute syntax, it can be used in front of shader properties to change the drawing method, more information can be found in the official documentation: https://docs.unity3d.com/ScriptReference/MaterialPropertyDrawer.html
   - Each Property can only have one Drawer
   - Each Property can have multiple Decorators

## Basic Drawers

### Main & Sub

```c#
/// Create a Folding Group
/// 
/// group: group name (Default: Property Name)
/// keyword: keyword used for toggle, "_" = ignore, none or "__" = Property Name +  "_ON", always Upper (Default: none)
/// default Folding State: "on" or "off" (Default: off)
/// default Toggle Displayed: "on" or "off" (Default: on)
/// preset File Name: "Shader Property Preset" asset name, see Preset() for detail (Default: none)
/// Target Property Type: Float, express Toggle value
public MainDrawer() : this(String.Empty) { }
public MainDrawer(string group) : this(group, String.Empty) { }
public MainDrawer(string group, string keyword) : this(group, keyword, "off") { }
public MainDrawer(string group, string keyword, string defaultFoldingState) : this(group, keyword, defaultFoldingState, "on") { }
public MainDrawer(string group, string keyword, string defaultFoldingState, string defaultToggleDisplayed) : this(group, keyword, defaultFoldingState, defaultToggleDisplayed, String.Empty) { }
public MainDrawer(string group, string keyword, string defaultFoldingState, string defaultToggleDisplayed, string presetFileName)

```

```c#
/// Draw a property with default style in the folding group
/// 
/// group: father group name (Default: none)
/// Target Property Type: Any
public SubDrawer() { }
public SubDrawer(string group)

```

Example:

```c#
[Title(Main Samples)]
[Main(GroupName)]
_group ("Group", float) = 0
[Sub(GroupName)] _float ("Float", float) = 0


[Main(Group1, _KEYWORD, on)] _group1 ("Group - Default Open", float) = 1
[Sub(Group1)] _float1 ("Sub Float", float) = 0
[Sub(Group1)] _vector1 ("Sub Vector", vector) = (1, 1, 1, 1)
[Sub(Group1)] [HDR] _color1 ("Sub HDR Color", color) = (0.7, 0.7, 1, 1)

[Title(Group1, Conditional Display Samples       Enum)]
[KWEnum(Group1, Name 1, _KEY1, Name 2, _KEY2, Name 3, _KEY3)]
_enum ("KWEnum", float) = 0

// Display when the keyword ("group name + keyword") is activated
[Sub(Group1_KEY1)] _key1_Float1 ("Key1 Float", float) = 0
[Sub(Group1_KEY2)] _key2_Float2 ("Key2 Float", float) = 0
[Sub(Group1_KEY3)] _key3_Float3_Range ("Key3 Float Range", Range(0, 1)) = 0
[SubPowerSlider(Group1_KEY3, 10)] _key3_Float4_PowerSlider ("Key3 Power Slider", Range(0, 1)) = 0

[Title(Group1, Conditional Display Samples       Toggle)]
[SubToggle(Group1, _TOGGLE_KEYWORD)] _toggle ("SubToggle", float) = 0
[Tex(Group1_TOGGLE_KEYWORD)][Normal] _normal ("Normal Keyword", 2D) = "bump" { }
[Sub(Group1_TOGGLE_KEYWORD)] _float2 ("Float Keyword", float) = 0


[Main(Group2, _, off, off)] _group2 ("Group - Without Toggle", float) = 0
[Sub(Group2)] _float3 ("Float 2", float) = 0

```

Default result:

![image-20220828003026556](assets~/image-20220828003026556.png)

Then change values:

![image-20220828003129588](assets~/image-20220828003129588.png)

## Extra Drawers

### Numeric

#### SubToggle

```c#
/// Similar to builtin Toggle()
/// 
/// group: father group name (Default: none)
/// keyword: keyword used for toggle, "_" = ignore, none or "__" = Property Name +  "_ON", always Upper (Default: none)
/// preset File Name: "Shader Property Preset" asset name, see Preset() for detail (Default: none)
/// Target Property Type: Float
public SubToggleDrawer() { }
public SubToggleDrawer(string group) : this(group, String.Empty, String.Empty) { }
public SubToggleDrawer(string group, string keyWord) : this(group, keyWord, String.Empty) { }
public SubToggleDrawer(string group, string keyWord, string presetFileName)
```



#### SubPowerSlider

```c#
/// Similar to builtin PowerSlider()
/// 
/// group: father group name (Default: none)
/// power: power of slider (Default: 1)
/// Target Property Type: Range
public SubPowerSliderDrawer(float power) : this("_", power) { }
public SubPowerSliderDrawer(string group, float power)
```


#### SubIntRange

```c#
/// Similar to builtin IntRange()
/// 
/// group: father group name (Default: none)
/// Target Property Type: Range
public SubIntRangeDrawer(string group)

```



#### MinMaxSlider

```c#
/// Draw a min max slider
/// 
/// group: father group name (Default: none)
/// minPropName: Output Min Property Name
/// maxPropName: Output Max Property Name
/// Target Property Type: Range, range limits express the MinMaxSlider value range
/// Output Min/Max Property Type: Range, it's value is limited by it's range
public MinMaxSliderDrawer(string minPropName, string maxPropName) : this("_", minPropName, maxPropName) { }
public MinMaxSliderDrawer(string group, string minPropName, string maxPropName)

```

Example:

```c#
[Title(MinMaxSlider Samples)]
[MinMaxSlider(_rangeStart, _rangeEnd)] _minMaxSlider("Min Max Slider (0 - 1)", Range(0.0, 1.0)) = 1.0
/*[HideInInspector]*/_rangeStart("Range Start", Range(0.0, 0.5)) = 0.0
/*[HideInInspector]*/[PowerSlider(10)] _rangeEnd("Range End PowerSlider", Range(0.5, 1.0)) = 1.0

```

Result:

![image-20220828003810353](assets~/image-20220828003810353.png)



#### KWEnum

```c#
/// Similar to builtin Enum() / KeywordEnum()
/// 
/// group: father group name (Default: none)
/// n(s): display name
/// k(s): keyword
/// v(s): value
/// Target Property Type: Float, express current keyword index
public KWEnumDrawer(string n1, string k1)
public KWEnumDrawer(string n1, string k1, string n2, string k2)
public KWEnumDrawer(string n1, string k1, string n2, string k2, string n3, string k3)
public KWEnumDrawer(string n1, string k1, string n2, string k2, string n3, string k3, string n4, string k4)
public KWEnumDrawer(string n1, string k1, string n2, string k2, string n3, string k3, string n4, string k4, string n5, string k5)
    
public KWEnumDrawer(string group, string n1, string k1)
public KWEnumDrawer(string group, string n1, string k1, string n2, string k2)
public KWEnumDrawer(string group, string n1, string k1, string n2, string k2, string n3, string k3)
public KWEnumDrawer(string group, string n1, string k1, string n2, string k2, string n3, string k3, string n4, string k4)
public KWEnumDrawer(string group, string n1, string k1, string n2, string k2, string n3, string k3, string n4, string k4, string n5, string k5)
```



#### SubEnum & SubKeywordEnum

```c#
// enumName: like "UnityEngine.Rendering.BlendMode"
public SubEnumDrawer(string group, string enumName) : base(group, enumName)

public SubEnumDrawer(string group, string n1, float v1, string n2, float v2)
public SubEnumDrawer(string group, string n1, float v1, string n2, float v2, string n3, float v3)
public SubEnumDrawer(string group, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4)
public SubEnumDrawer(string group, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5)
public SubEnumDrawer(string group, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6)
public SubEnumDrawer(string group, string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6, string n7, float v7)


public SubKeywordEnumDrawer(string group, string kw1, string kw2)
public SubKeywordEnumDrawer(string group, string kw1, string kw2, string kw3)
public SubKeywordEnumDrawer(string group, string kw1, string kw2, string kw3, string kw4)
public SubKeywordEnumDrawer(string group, string kw1, string kw2, string kw3, string kw4, string kw5)
public SubKeywordEnumDrawer(string group, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6)
public SubKeywordEnumDrawer(string group, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7)
public SubKeywordEnumDrawer(string group, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8)
public SubKeywordEnumDrawer(string group, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8, string kw9)

```



#### Preset

```c#
/// Popping a menu, you can select the Shader Property Preset, the Preset values will replaces the default values
/// 
/// group: father group name (Default: none)
///	presetFileName: "Shader Property Preset" asset name, you can create new Preset by
///		"Right Click > Create > LWGUI > Shader Property Preset" in Project window,
///		*any Preset in the entire project cannot have the same name*
/// Target Property Type: Float, express current keyword index
public PresetDrawer(string presetFileName) : this("_", presetFileName) {}
public PresetDrawer(string group, string presetFileName)

```

Example:

~~~c#
[Title(Preset Samples)]
[Preset(LWGUI_BlendModePreset)] _BlendMode ("Blend Mode Preset", float) = 0 
[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 2
[Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend", Float) = 1
[Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend", Float) = 0
[Toggle(_)]_ZWrite("ZWrite ", Float) = 1
[Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest", Float) = 4 // 4 is LEqual
[Enum(RGBA,15,RGB,14)]_ColorMask("ColorMask", Float) = 15 // 15 is RGBA (binary 1111)
    
``````
    
Cull [_Cull]
ZWrite [_ZWrite]
Blend [_SrcBlend] [_DstBlend]
ColorMask [_ColorMask]
~~~

Result:

The Property Value in the selected Preset will be the default value:

![image-20221122231655378](assets~/image-20221122231655378.png)![image-20221122231816714](assets~/image-20221122231816714.png)

##### Create Preset File

![image-20221122232307362](assets~/image-20221122232307362.png)

##### Edit Preset

![image-20221122232354623](assets~/image-20221122232354623.png)![image-20221122232415972](assets~/image-20221122232415972.png)![image-20221122232425194](assets~/image-20221122232425194.png)



#### BitMask

```C#
/// Draw the Int value as a Bit Mask.
/// Note:
///    - Currently only 8 bits are supported.
///    - Property Type must be 'Integer', not 'Int'.
///
/// group: father group name (Default: none)
/// bitDescription 7-0: Description of each Bit. (Default: none)
/// Target Property Type: Integer
public BitMaskDrawer() : this(string.Empty, null) { }
public BitMaskDrawer(string group) : this(group, null) { }
public BitMaskDrawer(string group, string bitDescription7, string bitDescription6, string bitDescription5, string bitDescription4, string bitDescription3, string bitDescription2, string bitDescription1, string bitDescription0) 
    : this(group, new List<string>() { bitDescription0, bitDescription1, bitDescription2, bitDescription3, bitDescription4, bitDescription5, bitDescription6, bitDescription7 }) { }

```

Example:
```C#
[BitMask(Preset)] _Stencil ("Stencil", Integer) = 0  
[BitMask(Preset, Left, Bit6, Bit5, Bit4, Description, Bit2, Bit1, Right)] _StencilWithDescription ("Stencil With Description", Integer) = 0
```
Result:
![](assets~/Pasted%20image%2020250321174432.png)

### Texture

#### Tex

```c#
/// Draw a Texture property in single line with a extra property
/// 
/// group: father group name (Default: none)
/// extraPropName: extra property name (Default: none)
/// Target Property Type: Texture
/// Extra Property Type: Color, Vector
/// Target Property Type: Texture2D
public TexDrawer() { }
public TexDrawer(string group) : this(group, String.Empty) { }
public TexDrawer(string group, string extraPropName)

```

Example:

```c#
[Main(Group3, _, on)] _group3 ("Group - Tex and Color Samples", float) = 0
[Tex(Group3, _color)] _tex_color ("Tex with Color", 2D) = "white" { }
[HideInInspector] _color (" ", Color) = (1, 0, 0, 1)
[Tex(Group3, _textureChannelMask1)] _tex_channel ("Tex with Channel", 2D) = "white" { }
[HideInInspector] _textureChannelMask1(" ", Vector) = (0,0,0,1)

// Display up to 4 colors in a single line
[Color(Group3, _mColor1, _mColor2, _mColor3)]
_mColor ("Multi Color", Color) = (1, 1, 1, 1)
[HideInInspector] _mColor1 (" ", Color) = (1, 0, 0, 1)
[HideInInspector] _mColor2 (" ", Color) = (0, 1, 0, 1)
[HideInInspector] [HDR] _mColor3 (" ", Color) = (0, 0, 1, 1)

```

Result:

![image-20220828003507825](assets~/image-20220828003507825.png)

#### Ramp

##### ShaderLab

```c#
/// Draw an unreal style Ramp Map Editor (Default Ramp Map Resolution: 512 * 2)
/// NEW: The new LwguiGradient type has both the Gradient and Curve editors, and can be used in C# scripts and runtime, and is intended to replace UnityEngine.Gradient
///
/// group: father group name (Default: none)
/// defaultFileName: default Ramp Map file name when create a new one (Default: RampMap)
/// rootPath: the path where ramp is stored, replace '/' with '.' (for example: Assets.Art.Ramps). when selecting ramp, it will also be filtered according to the path (Default: Assets)
/// colorSpace: switch sRGB / Linear in ramp texture import setting (Default: sRGB)
/// defaultWidth: default Ramp Width (Default: 512)
/// viewChannelMask: editable channels. (Default: RGBA)
/// timeRange: the abscissa display range (1/24/2400), is used to optimize the editing experience when the abscissa is time of day. (Default: 1)
/// Target Property Type: Texture2D
public RampDrawer() : this(String.Empty) { }
public RampDrawer(string group) : this(group, "RampMap") { }
public RampDrawer(string group, string defaultFileName) : this(group, defaultFileName, DefaultRootPath, 512) { }
public RampDrawer(string group, string defaultFileName, float defaultWidth) : this(group, defaultFileName, DefaultRootPath, defaultWidth) { }
public RampDrawer(string group, string defaultFileName, string rootPath, float defaultWidth) : this(group, defaultFileName, rootPath, "sRGB", defaultWidth) { }
public RampDrawer(string group, string defaultFileName, string rootPath, string colorSpace, float defaultWidth) : this(group, defaultFileName, rootPath, colorSpace, defaultWidth, "RGBA") { }
public RampDrawer(string group, string defaultFileName, string rootPath, string colorSpace, float defaultWidth, string viewChannelMask) : this(group, defaultFileName, rootPath, colorSpace, defaultWidth, viewChannelMask, 1) { }
public RampDrawer(string group, string defaultFileName, string rootPath, string colorSpace, float defaultWidth, string viewChannelMask, float timeRange)
```

Example:

```c#
[Ramp(_, RampMap, Assets.Art, 512)] _Ramp ("Ramp Map", 2D) = "white" { }
```

Result:

![image-20230625185730363](./assets~/image-20230625185730363.png)

You **must manually Save the edit results**, if there are unsaved changes, the Save button will display yellow.

**When you move or copy the Ramp Map, remember to move together with the .meta file, otherwise you will not be able to edit it again!**

##### C#

Example:

```c#
public class Test : MonoBehaviour
{
    public LwguiGradient lwguiGradientSrgb = new LwguiGradient();

    [LwguiGradientUsage(ColorSpace.Linear, LwguiGradient.ChannelMask.RGB, LwguiGradient.GradientTimeRange.TwentyFourHundred)]
    public LwguiGradient lwguiGradientLinear = new LwguiGradient();
}
```

Result:

![image-20240717104144821](./assets~/image-20240717104144821.png)![image-20240717104206365](./assets~/image-20240717104206365.png)

Default display settings can be set using the LwguiGradientUsage() Attribute.

##### Gradient Editor

The new LWGUI Gradient Editor integrates with Unity's built-in [Gradient Editor](https://docs.unity3d.com/Manual/EditingValueProperties.html) and [Curve Editor](https://docs.unity3d.com/Manual/EditingCurves.html), enabling more powerful features than UE's Gradient Editor.

![image-20241126110012922](./assets~/image-20241126110012922.png)

| Editor                | Description                                                  |
| --------------------- | ------------------------------------------------------------ |
| Time Range            | The display range of the horizontal axis, selectable from 0-1 / 0-24 / 0-2400, is very useful when the horizontal axis is time. Note that only the display is affected, the actual value stored in the horizontal axis is always 0-1. |
| Channels              | The channels displayed. Can be displayed individually.       |
| sRGB Preview          | It should be checked when the value of Gradient is a color to preview the correct color, otherwise it doesn't need to be checked. Only affects the display, Gradient and Ramp Map are always stored as Linear. |
| Value / R / G / B / A | Used to edit the Value of the selected Key, you can edit the Value of multiple Keys at the same time. |
| Time                  | Used to edit the Time of the selected Key, you can edit the Time of multiple Keys at the same time. If you enter a number manually, you must **press enter** to end the editing. |
| Gradient Editor       | This is similar to Unity's built-in [Gradient Editor](https://docs.unity3d.com/Manual/EditingValueProperties.html), but the Alpha channels are separated into black and white. <br/>Note that **adding keys from the Gradient Editor is limited to a maximum of 8 keys**, adding keys from the Curve Editor is **unlimited**. Exceeding the key limit will not affect preview or usage. |
| Curve Editor          | Similar to Unity's built-in Curve Editor, it displays the XY 0-1 range by default, and you can use the scroll wheel to zoom or move the display range.<br/>As you can see in the image below, the context menu has a number of functions for controlling the shape of the curve, and you can consult the [Unity documentation](https://docs.unity3d.com/Manual/EditingCurves.html) to get the most out of these functions. |
| Presets               | You can save the current LWGUI Gradient as a preset and apply it anytime. These presets are common between different engine versions on the local computer, but are not saved to the project. |

![image-20241126105823397](./assets~/image-20241126105823397.png)![image-20241126112320151](./assets~/image-20241126112320151.png)

**Known issues:**

- Preview images below Unity 2022 have no difference between sRGB/Linear color spaces
- Ctrl + Z results may be slightly different from expected when the editor frame rate is too low



#### Image

```c#
/// Draw an image preview.
/// display name: The path of the image file relative to the Unity project, such as: "assets~/test.png", "Doc/test.png", "../test.png"
/// 
/// group: father group name (Default: none)
/// Target Property Type: Any
public ImageDrawer() { }
public ImageDrawer(string group)
```

Result:

![image-20240416142736663](./assets~/image-20240416142736663.png)

### Vector

#### Color

```c#
/// Display up to 4 colors in a single line
/// 
/// group: father group name (Default: none)
/// color2-4: extra color property name
/// Target Property Type: Color
public ColorDrawer(string group, string color2) : this(group, color2, String.Empty, String.Empty) { }
public ColorDrawer(string group, string color2, string color3) : this(group, color2, color3, String.Empty) { }
public ColorDrawer(string group, string color2, string color3, string color4)

```

Example:

```c#
[Main(Group3, _, on)] _group3 ("Group - Tex and Color Samples", float) = 0
[Tex(Group3, _color)] _tex_color ("Tex with Color", 2D) = "white" { }
[HideInInspector] _color (" ", Color) = (1, 0, 0, 1)
[Tex(Group3, _textureChannelMask1)] _tex_channel ("Tex with Channel", 2D) = "white" { }
[HideInInspector] _textureChannelMask1(" ", Vector) = (0,0,0,1)

// Display up to 4 colors in a single line
[Color(Group3, _mColor1, _mColor2, _mColor3)]
_mColor ("Multi Color", Color) = (1, 1, 1, 1)
[HideInInspector] _mColor1 (" ", Color) = (1, 0, 0, 1)
[HideInInspector] _mColor2 (" ", Color) = (0, 1, 0, 1)
[HideInInspector] [HDR] _mColor3 (" ", Color) = (0, 0, 1, 1)

```

Result:

![image-20220828003507825](assets~/image-20220828003507825.png)



#### Channel

```c#
/// Draw a R/G/B/A drop menu:
/// 	R = (1, 0, 0, 0)
/// 	G = (0, 1, 0, 0)
/// 	B = (0, 0, 1, 0)
/// 	A = (0, 0, 0, 1)
/// 	RGB Average = (1f / 3f, 1f / 3f, 1f / 3f, 0)
/// 	RGB Luminance = (0.2126f, 0.7152f, 0.0722f, 0)
///		None = (0, 0, 0, 0)
/// 
/// group: father group name (Default: none)
/// Target Property Type: Vector, used to dot() with Texture Sample Value
public ChannelDrawer() { }
public ChannelDrawer(string group)
```

Example:

```c#
[Title(_, Channel Samples)]
[Channel(_)]_textureChannelMask("Texture Channel Mask (Default G)", Vector) = (0,1,0,0)

......

float selectedChannelValue = dot(tex2D(_Tex, uv), _textureChannelMask);
```



![image-20220822010511978](assets~/image-20220822010511978.png)



### Other

#### Button

```c#
/// Draw one or more Buttons within the same row, using the Display Name to control the appearance and behavior of the buttons
/// 
/// Declaring a set of Button Name and Button Command in Display Name generates a Button, separated by '@':
/// ButtonName0@ButtonCommand0@ButtonName1@ButtonCommand1
/// 
/// Button Name can be any other string, the format of Button Command is:
/// TYPE:Argument
/// 
/// The following TYPEs are currently supported:
/// - URL: Open the URL, Argument is the URL
/// - C#: Call the public static C# function, Argument is NameSpace.Class.Method(arg0, arg1, ...),
///		for target function signatures, see: LWGUI.ButtonDrawer.TestMethod().
///
/// The full example:
/// [Button(_)] _button0 ("URL Button@URL:https://github.com/JasonMa0012/LWGUI@C#:LWGUI.ButtonDrawer.TestMethod(1234, abcd)", Float) = 0
/// 
/// group: father group name (Default: none)
/// Target Property Type: Any
public ButtonDrawer() { }
public ButtonDrawer(string group)
```

Example:

```c#
[Title(Button Samples)]
[Button(_)] _button0 ("URL Button@URL:https://github.com/JasonMa0012/LWGUI@C# Button@C#:LWGUI.ButtonDrawer.TestMethod(1234, abcd)", Float) = 0

```

![image-20241127180711449](./assets~/image-20241127180711449.png)





## Extra Decorators

### Appearance

#### Title & SubTitle

```c#
/// <summary>
/// Similar to Header()
/// 
/// group: father group name (Default: none)
/// header: string to display, "SpaceLine" or "_" = none (Default: none)
/// height: line height (Default: 22)
public TitleDecorator(string header) : this("_", header, DefaultHeight) {}
public TitleDecorator(string header, float  height) : this("_", header, height) {}
public TitleDecorator(string group,  string header) : this(group, header, DefaultHeight) {}
public TitleDecorator(string group, string header, float height)


/// Similar to Title()
/// 
/// group: father group name (Default: none)
/// header: string to display, "SpaceLine" or "_" = none (Default: none)
/// height: line height (Default: 22)
public SubTitleDecorator(string group,  string header) : base(group, header, DefaultHeight) {}
public SubTitleDecorator(string group, string header, float height) : base(group, header, height) {}

```

#### Tooltip & Helpbox

```c#
/// Tooltip, describes the details of the property. (Default: property.name and property default value)
/// You can also use "#Text" in DisplayName to add Tooltip that supports Multi-Language.
/// 
/// tooltip: a single-line string to display, support up to 4 ','. (Default: Newline)
public TooltipDecorator() : this(string.Empty) {}
public TooltipDecorator(string tooltip) { this._tooltip = tooltip; }
public TooltipDecorator(string s1, string s2) : this(s1 + ", " + s2) { }
public TooltipDecorator(string s1, string s2, string s3) : this(s1 + ", " + s2 + ", " + s3) { }
public TooltipDecorator(string s1, string s2, string s3, string s4) : this(s1 + ", " + s2 + ", " + s3 + ", " + s4) { }
public TooltipDecorator(string s1, string s2, string s3, string s4, string s5) : this(s1 + ", " + s2 + ", " + s3 + ", " + s4 + ", " + s5) { }


```

```c#
/// Display a Helpbox on the property
/// You can also use "%Text" in DisplayName to add Helpbox that supports Multi-Language.
/// 
/// message: a single-line string to display, support up to 4 ','. (Default: Newline)
public HelpboxDecorator() : this(string.Empty) {}
public HelpboxDecorator(string message) { this._message = message; }
public HelpboxDecorator(string s1, string s2) : this(s1 + ", " + s2) { }
public HelpboxDecorator(string s1, string s2, string s3) : this(s1 + ", " + s2 + ", " + s3) { }
public HelpboxDecorator(string s1, string s2, string s3, string s4) : this(s1 + ", " + s2 + ", " + s3 + ", " + s4) { }
public HelpboxDecorator(string s1, string s2, string s3, string s4, string s5) : this(s1 + ", " + s2 + ", " + s3 + ", " + s4 + ", " + s5) { }


```

Example:

```c#
[Title(Metadata Samples)]
[Tooltip(Test multiline Tooltip, a single line supports up to 4 commas)]
[Tooltip()]
[Tooltip(Line 3)]
[Tooltip(Line 4)]
_float_tooltip ("Float with Tooltips#这是中文Tooltip#これは日本語Tooltipです", float) = 1
[Helpbox(Test multiline Helpbox)]
[Helpbox(Line2)]
[Helpbox(Line3)]
_float_helpbox ("Float with Helpbox%这是中文Helpbox%これは日本語Helpboxです", float) = 1

```

![image-20221231221240686](assets~/image-20221231221240686.png)

![image-20221231221254101](assets~/image-20221231221254101.png)

Tips:

- Tooltip may disappear when the Editor is running. This is a feature of Unity itself (or a bug)



#### ReadOnly

```c#
/// Set the property to read-only.
public ReadOnlyDecorator()
```



### Logic

#### PassSwitch

```c#
/// Cooperate with Toggle to switch certain Passes.
/// 
/// lightModeName(s): Light Mode in Shader Pass (https://docs.unity3d.com/2017.4/Documentation/Manual/SL-PassTags.html)
public PassSwitchDecorator(string   lightModeName1) 
public PassSwitchDecorator(string   lightModeName1, string lightModeName2) 
public PassSwitchDecorator(string   lightModeName1, string lightModeName2, string lightModeName3) 
public PassSwitchDecorator(string   lightModeName1, string lightModeName2, string lightModeName3, string lightModeName4) 
public PassSwitchDecorator(string   lightModeName1, string lightModeName2, string lightModeName3, string lightModeName4, string lightModeName5) 
public PassSwitchDecorator(string   lightModeName1, string lightModeName2, string lightModeName3, string lightModeName4, string lightModeName5, string lightModeName6) 

```



### Structure



#### Advanced & AdvancedHeaderProperty

```c#
/// Collapse the current Property into an Advanced Block.
/// Specify the Header String to create a new Advanced Block.
/// All Properties using Advanced() will be collapsed into the nearest Advanced Block.
/// 
/// headerString: The title of the Advanced Block. Default: "Advanced"
public AdvancedDecorator() : this(string.Empty) { }
public AdvancedDecorator(string headerString)
```

```c#
/// Create an Advanced Block using the current Property as the Header.
public AdvancedHeaderPropertyDecorator()
```

Example:

```c#
[Main(Group2, _, off, off)] _group2 ("Group - Without Toggle", float) = 0
[Sub(Group2)] _float3 ("Float 2", float) = 0
[Advanced][Sub(Group2)] _Advancedfloat0 ("Advanced Float 0", float) = 0
[Advanced][Sub(Group2)] _Advancedfloat1 ("Advanced Float 1", float) = 0
[Advanced(Advanced Header Test)][Sub(Group2)] _Advancedfloat3 ("Advanced Float 3", float) = 0
[Advanced][Sub(Group2)] _Advancedfloat4 ("Advanced Float 4", float) = 0
[AdvancedHeaderProperty][Tex(Group2, _Advancedfloat7)] _AdvancedTex0 ("Advanced Header Property Test", 2D) = "white" { }
[Advanced][HideInInspector] _Advancedfloat7 ("Advanced Float 7", float) = 0
[Advanced][Tex(Group2, _AdvancedRange0)] _AdvancedTex1 ("Advanced Tex 1", 2D) = "white" { }
[Advanced][HideInInspector] _AdvancedRange0 ("Advanced Range 0", Range(0, 1)) = 0

```

![image-20231007163044176](./assets~/image-20231007163044176.png)

Tips:

- LWGUI uses a tree data structure to store the relationship between Group, Advanced Block and their children. In theory, it can store unlimited multi-level parent-child relationships, but **currently LWGUI only manually handles 3-level parent-child relationships, which means you can put an Advanced Block in a Group, but a Group cannot be placed in an Advanced Block.**



### Condition Display

#### Hidden

```c#
/// Similar to HideInInspector(), the difference is that Hidden() can be unhidden through the Display Mode button.
public HiddenDecorator()
```



#### ShowIf

```c#
/// Control the show or hide of a single or a group of properties based on multiple conditions.
///
/// logicalOperator: And | Or (Default: And).
/// propName: Target Property Name used for comparison.
/// compareFunction: Less (L) | Equal (E) | LessEqual (LEqual / LE) | Greater (G) | NotEqual (NEqual / NE) | GreaterEqual (GEqual / GE).
/// value: Target Property Value used for comparison.
public ShowIfDecorator(string propName, string comparisonMethod, float value) : this("And", propName, comparisonMethod, value) { }
public ShowIfDecorator(string logicalOperator, string propName, string compareFunction, float value)
```

Example:

```c#
[ShowIf(_enum, Equal, 1)]
[Title(ShowIf Main Samples)]
[Main(GroupName)] _group ("Group", float) = 0
[Sub(GroupName)] _float ("Float", float) = 0
[Sub(GroupName)] _Tex ("Tex", 2D) = "white" { }

...

[SubTitle(Group1, Conditional Display Samples       Enum)]
[KWEnum(Group1, Name 1, _KEY1, Name 2, _KEY2, Name 3, _KEY3)] _enum ("KWEnum", float) = 0
[Sub(Group1)][ShowIf(_enum, Equal, 0)] _key1_Float1 ("Key1 Float", float) = 0
[Sub(Group1)][ShowIf(_enum, Equal, 1)] _key2_Float2 ("Key2 Float", float) = 0
[SubIntRange(Group1)][ShowIf(_enum, Equal, 2)] _key3_Int_Range ("Key3 Int Range", Range(0, 10)) = 0
[ShowIf(_enum, Equal, 0)][ShowIf(Or, _enum, Equal, 2)]
[SubPowerSlider(Group1, 3)] _key13_PowerSlider ("Key1 or Key3 Power Slider", Range(0, 1)) = 0

```

![image-20231023010137495](./assets~/image-20231023010137495.png)

![image-20231023010153213](./assets~/image-20231023010153213.png)

![image-20231023010204399](./assets~/image-20231023010204399.png)

## LWGUI Timeline Tracks

### MaterialKeywordToggleTrack

When recording material parameter animation, Keyword changes are automatically captured and the track is added to the Timeline Asset. The Keyword state is set according to the float value during runtime.

Supports Toggle-type Drawer with Keyword.



## Unity Builtin Drawers

### Space

```c#
MaterialSpaceDecorator(float height)
```

### Header

```c#
MaterialHeaderDecorator(string header)
```

### Enum

```c#
MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4, string n5, float v5, string n6, float v6, string n7, float v7)
```



### IntRange

```c#
MaterialIntRangeDrawer()
```



### KeywordEnum

```c#
MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8, string kw9)
```



### PowerSlider

```c#
MaterialPowerSliderDrawer(float power)
```



### Toggle

```c#
MaterialToggleUIDrawer(string keyword)
```



## Custom Shader GUI

### Custom Header and Footer

![image-20230821211652918](./assets~/image-20230821211652918.png)

1. Custom Headers and Footers enable you to integrate bespoke modules at the top or bottom of the ShaderGUI without altering LWGUI plugin code.

2. Depending on the desired location for the custom GUI, duplicate the following script into an Editor folder within your project:
   - Top: Packages/com.jasonma.lwgui/Editor/CustomGUISample/CustomHeader.cs
   - Bottom: Packages/com.jasonma.lwgui/Editor/CustomGUISample/CustomFooter.cs
3. Modify the file name and class name accordingly.
4. Implement your custom GUI code within the DoCustomHeader() / DoCustomFooter() functions.
5. It is advisable to examine the lwgui object definition to obtain any requisite data.

### Custom Drawer

TODO



## Contribution

1. Create multiple empty projects using different versions of Unity
2. Pull this repo
3. Use symbolic links to place this repo in the Assets or Packages directory of all projects
4. Inherit the `Subdrawer` in` shadeerdrawer.cs` to start developing your custom Drawer
5. Check whether the functionality works in different Unity versions
6. Pull requests







