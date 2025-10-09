# Quick Start Guide - Making Scenes Visible in Unity

## Problem: GameObjects Not Saving Between Scene Switches

When you create GameObjects using the Editor menu tools, they must be **saved** to the scene file. The updated `SceneSetupHelper` now automatically saves scenes after creating GameObjects.

---

## Quick Setup Steps (5 Minutes)

### 1. Open Unity Editor
- Open the `storybook` project in Unity Editor (Unity 6)
- Wait for scripts to compile (check bottom-right corner)

### 2. Setup Home Scene

1. **Open** `Assets/Scenes/Home.unity` (double-click in Project window)
2. **Menu**: `Tools > Emotion Maze > Setup Home Scene`
3. Click **"Yes"** in the dialog
4. **Wait** for console message: `"Home scene setup complete!"`
5. **Verify** in Hierarchy:
   - `_GameManagers` (with GameManager + NavigationManager children)
   - `UI Canvas` (with Background, ProgressIndicator, HomeButton, 4 Portals)
   - `EventSystem`
   - `Moro` (sprite in world space)

**Scene is now saved!** ✅

### 3. Add Scenes to Build Settings

**Menu**: `Tools > Emotion Maze > Add All Scenes to Build Settings`

Check: `File > Build Settings` - Should show 6 scenes

### 4. Setup Maze Scenes (Optional - Repeat 4 Times)

For each maze scene (`Maze_Anger.unity`, `Maze_Sadness.unity`, etc.):

1. **Open** the maze scene
2. **Menu**: `Tools > Emotion Maze > Setup Maze Scene`
3. Click **"Yes"**
4. **Verify** scene is saved

### 5. Setup Credits Scene

1. **Open** `Assets/Scenes/Credits.unity`
2. **Menu**: `Tools > Emotion Maze > Setup Credits Scene`
3. Click **"Yes"**

---

## Test in Play Mode

### From Home Scene:

1. Open `Home.unity`
2. Press **Play** button (or F5)
3. **You should see:**
   - Cream/beige background
   - Teal circle in center (Moro)
   - 4 colored circles around it:
     - Top-left: Red (Anger)
     - Top-right: Blue (Sadness)
     - Bottom-left: Purple (Fear)
     - Bottom-right: Yellow (Joy)
   - 4 small grey dots at bottom
   - Home icon top-left

4. **Click a portal** (e.g., Red Anger portal):
   - Scene should transition to `Maze_Anger`
   - Background turns dark red
   - Dot at bottom lights up in red
   - Home button appears

5. **Click Home button**:
   - Returns to Home scene
   - Moro turns teal again

---

## Troubleshooting

### GameObjects Disappear When Switching Scenes

**Solution**: The updated `SceneSetupHelper.cs` now includes:
```csharp
MarkSceneDirty();      // Tells Unity the scene changed
SaveCurrentScene();    // Saves the .unity file
```

If you created objects before this fix:
1. Delete all GameObjects in the scene
2. Run the setup tool again
3. Scene will be saved properly this time

### "NullReferenceException" Errors

**Cause**: Script references not assigned

**Fix**: Check these in Inspector:
- `ProgressIndicator` → All 4 dot image slots filled
- `EmotionPortalUI` → Portal Image and Emotion Type set
- `HomeButtonUI` → No required references (should work)

### Scenes Not Loading

**Check**:
1. `File > Build Settings` - All 6 scenes listed?
2. Scene names match exactly (case-sensitive):
   - `Home`
   - `Maze_Anger`
   - `Maze_Sadness`
   - `Maze_Fear`
   - `Maze_Joy`
   - `Credits`

### Buttons Don't Respond to Clicks

**Check**:
- `EventSystem` exists in Hierarchy
- `UI Canvas` has `GraphicRaycaster` component
- Buttons have `Button` component

---

## Current Visual State (Placeholders)

Right now everything uses simple geometric shapes:

| Element | Current Look | Future (Phase 2-3) |
|---------|--------------|-------------------|
| Moro | Teal circle sprite | Hand-drawn fuzzy monster |
| Portals | Colored circles | Pulsing emotion auras |
| Background | Solid colors | Illustrated environments |
| Dots | Small grey circles | Custom progress indicators |
| Home Button | "🏠" text | Cloud-shaped sprite |

---

## Manual Verification Checklist

After running setup tools, verify these files were modified:

```bash
# Check if scenes were saved (should show recent timestamp)
ls -lh Assets/Scenes/Home.unity
ls -lh Assets/Scenes/Maze_Anger.unity
ls -lh Assets/Scenes/Credits.unity
```

Should show modification time = when you ran the tool.

---

## What's Working Now (Phase 1 Complete)

✅ Scene structure created
✅ Core scripts (GameManager, NavigationManager)
✅ Moro color changing system
✅ Scene navigation (Home ↔ Mazes ↔ Credits)
✅ Progress tracking (4 dots)
✅ UI components (buttons, portals)
✅ **Scenes save properly**

## What's Next (Phase 2-3)

🔲 Replace placeholder sprites with artwork
🔲 Add proper UI styling (rounded corners, shadows)
🔲 Implement Home scene interactions
🔲 Add Moro animations (breathing, expressions)
🔲 Polish transitions

---

## Getting Actual Sprites/Artwork

Currently using Unity's built-in sprites (`UI/Skin/Knob.psd`).

To add real artwork:

1. **Import sprites**:
   - Create folder: `Assets/Art/Characters/`, `Assets/Art/UI/`
   - Drag PNG/PSD files into these folders
   - Select each → Inspector → Texture Type: `Sprite (2D and UI)`

2. **Replace in GameObjects**:
   - Select `Moro` in Hierarchy
   - Sprite Renderer → Sprite: Drag your Moro sprite here
   - Repeat for portal images, buttons, etc.

3. **Reference artwork** in `LMfig/` folder for visual designs

---

## Need Help?

**Console errors?** Check:
- All scripts compiled without errors
- GameManager and NavigationManager exist in Home scene
- All UI components have required script references

**Scenes still not persisting?**
- Make sure you ran the setup **after** the fix
- Check Unity Console for "Saved scene: [name]" messages
- Try: `File > Save` (Ctrl+S) manually after running tool

**Everything working?**
- Proceed to Phase 2 (UI Framework) or Phase 3 (Home Page Polish)
