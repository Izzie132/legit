# QQ Project Name

## Description

This is a template project created using .NET and React.

## Documentation

You can find documentation for this project in the `Documentation` folder. Some key links can be found below:

- [QQ Project Name](Documentation/QQ-Project-Name.md)
- [Zero to Hero](Documentation/Zero-To-Hero.md)

## New project setup

To set up a new project:

1. Create a new repo in Azure DevOps
   - Create a new project
   - Create a new repo
   - Enter <your-new-project-name>
   - Click Create
   - Copy the clone URL

2. (Optional) Change your default branch name for new git repositories to `main`

   ```
   git config --global init.defaultBranch main
   ```

3. Clone the template, commit it as a new git repository, and push it to your new DevOps project

   ```
   git clone https://ghyston.visualstudio.com/GhystonTemplates/_git/dotnet-react <your-new-project-name>
   cd <your-new-project-name>
   rm -rf .git
   git init
   git add .
   git commit -m "Set up project from template"
   git remote add origin <new-project-clone-url>
   git push -u origin main
   ```

4. Rename the project
   - Open the project in Rider
   - Replace `QQProjectName` with your project name (without spaces)
     - In Rider, open the "Replace in Files" dialog with Ctrl+Shift+R
     - Tick "Include non-solution items"
     - Enable the "Preserve case" option on the very right
     - Replace `QQProjectName` with the new name of your project & click Replace All
   - Replace the other forms of the project name in the same way
     - Replace `QQ Project Name` with your project name separated by spaces
     - Replace `QQ-Project-Name` with your project name separated by hyphens
     - Replace `PRJCT` with a 5-character abbreviation of your project name
   - Also manually rename any files that have `QQ` in the name. This will include
     - `QQ-Project-Name.md`
     - `QQProjectName.sln`
     - `QQProjectName.bicep`
   - Commit & push the change

5. Set up the pipelines
   - In Azure, click Pipelines in the sidebar and then "New pipeline" in the top-right
   - For "Where is your code?" click "Azure Repos Git" and choose your new repository
   - For "Configure your pipeline" click "Existing Azure Pipelines YAML file"
     - Select the pipeline at `/Tools/Pipelines/build-and-test.yaml`
   - Add the SonaType variables so that the audit task will work
     - Click Variables
     - Add a new variable called `SONATYPE_OSS_INDEX_USERNAME` using the value from the "SonaType OSS Index Credentials (Project Template)" note in 1Password
     - Add another variable called `sonatypeOssIndexApiToken` using the value from 1Password, and select the "Keep this value secret" option
       (the variable name is in a different format due to the way that secret variables are handled)
   - Save it
   - Rename it (you can't set the name when creating it :disappointed:)
     - Go back to Pipelines in the sidebar and click the All tab at the top
     - Click the 3 dots to the right of your new pipeline, then "Rename/move"
     - Name it "QQProjectName - Build and Test"
   - Do the same with the `/Tools/Pipelines/build-and-deploy.yaml` file to make another pipeline called "QQProjectName - Build and Deploy"

6. Set up branch policies to require PRs to fit some conditions before they can be merged
   - In Azure, go to Repos -> Branches
   - Click the three dots on the right of the `main` branch, and go to branch policies
   - We don't want to enable "Require a minimum number of reviewers" because this will prevent Renovate PRs from getting merged automatically (see below)
   - Enable "Check for comment resolution" and set it to Required
   - Enable "Limit merge types" and un-check everything except "Squash merge"
   - Add a build validation step to run your new Build and Test pipeline
   - Add automatically included reviewers
     - Set the reviewer to be a "team" containing everyone who should be allowed to approve PRs
     - Make sure the team is added as a "Required" approver
     - Set the affected folders to `*; !**/package.json; !**/package-lock.json; !**/*.csproj; !**/Directory.Packages.props; !**/global.json`
       - This allows Renovate to bypass this requirement if it wants to, so that PRs to update certain dependencies will get merged automatically.
         Not all Renovate PRs will get automatically merged, and they will still need to have a passing build.
     - Un-check "Allow requestors to approve their own changes"

7. Set up new environments, as described in [New-Environment-Setup.md](Documentation/Build-And-Deploy/New-Environment-Setup.md)

8. Remove this "New project setup" part of the README, since it's no longer relevant to projects which have already been set up
