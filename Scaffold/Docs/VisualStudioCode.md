# Visual Studio Code Support #

Included in the root directory of this project is a folder named `.vscode` which stores configuration information to make your life developing in Visual Studio Code (vscode) a little bit more easier.

## Launch Configuration ##

The default Launch Configuration for this project in vscode has been configured to start a PostgreSQL database when you start a Debug session with the Web API and to stop it when the Debug session is over. For this to work, you'll need to have Docker installed.

To start a Debug session, simply press `F5` in vscode.

## Tasks ##

To support the Launch Configuration described above, various tasks have been defined in the `tasks.json` file in the `.vscode` folder. These tasks can be run independently by going to the *Terminal* menu in vscode and then selecting *Run Task...*

## Workspace Settings ##

Workspace Settings in vscode are shared vscode settings in a project. Please feel free to add them if it benefits the project.
