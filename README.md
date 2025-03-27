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

# 🧠 FDF Remapper – Initial Working Version (VB.NET / .NET 2.0)

This is the first working version of a console application that remaps field names in `.fdf` form files using mappings defined in a local Access database (`.mdb`).

It was developed for structured form processing workflows (e.g., clinical trial PDFs, medical case report forms), and supports both 1:1 and 1:N mappings.

---

## ✅ Key Features in This Version

- ✅ Field name mapping via Access `.mdb` file
- ✅ Supports both **1:1** (direct) and **1:N** (value-based) mappings
- ✅ Outputs a log file indicating successful replacements
- ❌ No log entries for unchanged lines yet
- ❌ No support for keys with suffixes (e.g., `FieldName_2`)
- 🖥️ Designed to run on .NET Framework 2.0

---

## 🧾 Sample Log Output

> Note: Lines that are not changed are **not logged** in this version.

```
Line 9: [1:1] Replaced 'FieldA' with 'StandardFieldA'
Line 10: [1:N] Replaced 'FieldB' with 'StandardFieldB' (Value: ValueX)
Line 11: [1:1] Replaced 'FieldC' with 'StandardFieldC'
```

---

## 🧪 How It Works

1. The tool loads a mapping table from a local Access `.mdb` file.
2. It processes the `.fdf` file line by line.
3. If a matching mapping is found:
   - For **1:1**, the field name is replaced directly.
   - For **1:N**, the replacement is based on matching the field’s value.
4. Replacements are logged line by line in a `.log` file.

---

## ⚙️ Usage

### Edit `Program.vb` with correct file paths:

```vbnet
Dim dbPath As String = "C:\YourPath\Mapping.mdb"
Dim fdfFilePath As String = "C:\Input\YourFile.fdf"
Dim outputPath As String = "C:\Output\ModifiedFile.fdf"
Dim formname As String = "FormNameUsedInMapping"
```

### Compile using Visual Studio or command line:

```bash
vbc /target:exe /out:FDFRemapper.exe Program.vb
```

### Run:

The tool will generate:
- A modified `.fdf` file with replaced field names
- A `.log` file showing the lines that were changed

---

## 📌 Roadmap

Planned improvements for future versions:

- [x] Add `[NO CHANGE]` logging
- [x] Add suffix-based key handling (`Field_2`, `Field_3`)
- [ ] GUI support
- [ ] Batch mode for processing multiple `.fdf` files
- [ ] Web version (ASP.NET)

---

## 🧳 Compatibility

- VB.NET (.NET Framework 2.0)
- Works on Windows XP, Windows 7

