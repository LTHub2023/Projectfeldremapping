
# 🧠 FDF Remapper（fdfFieldReplacer） – Stage 4 Version (VB.NET / .NET 2.0)

A VB.NET 2.0 compatible tool for safely remapping field names in `.fdf` files using mappings from an Access `.mdb` database.

This tool was originally developed under constrained company workstation conditions (Windows XP, Notepad++ v6.8, no IDE) and later brought into Git version control manually. Therefore, early stages were linear and patched retrospectively.

---

## ✅ Current Features (Latest Stage: `stage4`)

- ✅ 1:1 and 1:N field remapping based on Access DB mappings
- ✅ Suffix support (e.g., `FieldName_2`, `FieldName_3`) works on **all fields**, not only selected keys
- ✅ `[NO CHANGE]` logging for unmatched tags
- ✅ Console-based `debugMode` for step-by-step output
- ✅ File path validation: ensures both `.fdf` and `.mdb` exist before execution
- ✅ Compatible with `.NET Framework 2.0`, executable on XP/Win7 machines

---
🔧 Modular refactoring (multi-file architecture) and future GUI integrations are planned for separate development branches and are not included in the main stage4 tag.

---

## 📜 Development History (Version Tags)

| Tag        | Description |
|------------|-------------|
| `stage1`   | Basic 1:1 and 1:N field remapping, no logging for unchanged |
| `stage2`   | Introduced `[NO CHANGE]` log entries |
| `stage3`   | Added regex-based suffix matching for **all fields** (not only `DocNum`, `VisNr`, `UE_Nr`) |
| `stage4`(current)   | Added `debugMode` and file path validation. Final version before modular restructuring |
 
 The project was version-tagged manually after development, and legacy `.vb` files have been archived in `/Legacy_Stages/`

## 🧾 Sample Log Output

```
Line 10: [1:1] Replaced 'Field' with 'StandardField'
Line 13: [1:1] Replaced 'Field_3' with 'StandardField_3' 
Line 12: [1:N] Replaced 'Visit' with 'VISITID' (Value: Screening)
Line 14: [NO CHANGE] Tag found in line: Unknown_1
Line 15: [NO CHANGE] no tag in line
```

---

## ⚙️ Usage

### Edit paths and form name in `Program.vb`:

```vbnet
Dim dbPath As String = "C:\Path\To\Mapping.mdb"
Dim fdfFilePath As String = "C:\Input\FormFile.fdf"
Dim outputPath As String = "C:\Output\ModifiedFile.fdf"
Dim formname As String = "FormNameUsedInMapping"
```

### Compile using VB.NET Compiler

```bash
vbc /target:exe /out:FDFRemapper.exe Program.vb
```

---

## 🧠 How It Works

1. Loads all mapping entries for a specific form from Access `.mdb`
2. Parses each line in `.fdf`
3. Replaces matching tags using:
   - Exact key match (1:1)
   - Value match (1:N)
   - Regex for suffix detection (e.g., `_2`, `_3`) is automatically applied across all fields that exist in the mapping table.
4. Logs every change, and unmatched tags

---

## 🧭 Roadmap

- [x] 1:1 and 1:N mapping logic
- [x] `[NO CHANGE]` logging
- [x] Suffix-based mapping with whitelist
- [x] Console-based `debugMode` & File path validation
- [ ] Split into components
- [ ] GUI (WinForms)
- [ ] Batch `.fdf` folder processing
- [ ] ASP.NET internal tool

---

## 🧳 Compatibility

- Built with VB.NET (.NET Framework 2.0)
- Works on Windows XP, Windows 7

---

## 🔧 Maintainer's Comment

This utility was initially created as a production aid, then reverse-documented and split into logical stages for GitHub publication.  
Stage4 represents the last monolithic version before structural improvements are handed off to future contributors or new branches.
