# Projectfeldremapping
# VB.NET tool to remap FDF field names from Access DB


Just upload some recent works

Earlier, I worked in China and used Gitlab with a company account.
Since last July, when I returned to Germany, people have suggested I upload some Works regularly.
As an Automobile engineer, I didn't need to do so, as I usually use Matlab/Simulink. However, at the last important position, I was requested not to write code but to build models in Simulink at Li Auto. 
It was a research and development project. X-by-wire, Model-based Design use dSPACE tools to generate embedded C Code and run on the MAB II as a controller. It is more like agile dev. rather than ASPACE-V. 
After one year, the project was to transition into mass production. 
When I studied at the university in Germany, the students in my major usually just took exams and did experiments.  I wanted to do some projects in the lab or testing ground, which I found cool. Finally, I had a chance to do a simulation.
At that time, typical automobile engineering was not that much like software engineering right now. Simulation and real-time control was most job dealing with software for sutdents under automobile engineering.

Now, I have just come back to Germany and recently work as a Database manager for a 5-employee company, but for the transition phase, I had to get a resident permit. 
I still want to seek opportunities with Better conditions.

This Project is a remapping tool for .fdf files.
Field names should be replaced. We have a mapping in the database, and we need to adjust the field name in the FDF Files. Instead of manually find and replace, I decided to write a code to make it automatic.

However, the company only gave me a pc with OS WinXP  and a lot of limitations, installing dev.envr. is not allowed... I had to use VB.NET and the work on .NET 2.0.......no Python is allowed
Although  it is a legacy database, the others in the company can use Win7(still old, but at least they can use relatively modern devtools)


---


# 🧠 FDF Remapper – Stage 2 Version (VB.NET / .NET 2.0)

This is the second-stage version of a VB.NET console application that remaps field names in `.fdf` form files based on mapping definitions in a local Access database (`.mdb`).  

In this stage, the application introduces **more complete logging**, including for fields that are not changed during remapping.

---

## ✅ Improvements from Stage 1

- ✅ Clear and readable log format
- ✅ Added `[NO CHANGE]` entries to the log for:
  - Known tags that do not match any mapping
  - Lines without any tag at all
- ✅ Still supports both:
  - **1:1 mapping** (no value required)
  - **1:N mapping** (based on `Value:` field content)
- ❌ Does **not** support fields with suffixes like `Field_2`, `Field_3` yet

---

## 🧾 Sample Log Output

```
Line 5: [1:1] Replaced 'Field1' with 'Field1remapping'
Line 6: [1:N] Replaced 'Field2' with 'Field2mapped' (Value: example)
Line 7: [NO CHANGE] Tag found in line: <p><b>UnknownTag</b>&
Line 8: [NO CHANGE] no tag in line
```

---

## 🛠 How It Works

1. Loads a mapping table from Access `.mdb` using `FormName` as a filter
2. Reads an `.fdf` file line-by-line
3. Applies:
   - 1:1 mapping when no value condition is set
   - 1:N mapping when `Value:` is used and matches `MC_Wert`
4. Records changes and unmatched lines to a `.log` file

---

## ⚙️ Usage Instructions

### Edit `Program.vb` with correct file paths:

```vbnet
Dim dbPath As String = "C:\YourPath\Mapping.mdb"
Dim fdfFilePath As String = "C:\Input\YourFile.fdf"
Dim outputPath As String = "C:\Output\ModifiedFile.fdf"
Dim formname As String = "FormName"
```

### Compile:

```bash
vbc /target:exe /out:FDFRemapper_Stage2.exe Program.vb
```

---

## 🧭 Roadmap

- [x] Add `[NO CHANGE]` logging ✔
- [ ] Handle `FieldName_2`, `FieldName_3`, etc. using pattern matching
- [ ] GUI version (WinForms)
- [ ] Folder batch processing
- [ ] Web tool (ASP.NET)

---

## 🧳 Compatibility

- VB.NET (.NET Framework 2.0)
- Works on Windows XP, Windows 7