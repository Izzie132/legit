# Project Name

## Description

This is a template project created using .NET and React.

## Documentation

You can find documentation for this project in the `Documentation` folder. Some key links can be found below:

- [Project Name](Documentation/Project-Name.md)
- [Zero to Hero](Documentation/Zero-To-Hero.md)

## New project setup

To set up a new project:

1. Clone the template

```
git clone https://ghyston.visualstudio.com/GhystonTemplates/_git/dotnet-react <your-new-project-name>
```

2. Navigate to the project directory

```
cd <your-new-project-name>
```

3. Remove the existing git repository link

```
# Linux/macOS/Git Bash
rm -rf .git

# Windows Command Prompt
rmdir /s .git

# Windows PowerShell
Remove-Item -Recurse -Force .git
```

4. Initialize a new git repository

```
git init
```

5. Add all files to git

```
git add .
```

6. Create an initial commit

```
git commit -m "Set up project from template"
```

7. Create a new repo in Azure DevOps
   1. Create a new project
   2. Create a new repo
   3. Enter <your-new-project-name>
   4. Uncheck "Add a README" to create an empty repository
   5. Click Create
   6. Copy the clone URL

8. Add DevOps remote

```
git remote add origin <new-project-clone-url>
```

9. Set default branch (if needed)

```
git branch -M main
```

10. Push to DevOps

```
git push -u origin main
```

Setting up branch policies to use [build-and-test.yaml](Tools/Pipelines/build-and-test.yaml) will require the SonaType OSS Index Credentials environments variables in the pipeline found in 1Password.
