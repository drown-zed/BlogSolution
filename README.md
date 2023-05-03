# Blog

## About project
It's a simple blog project, where blog posts can be generated with ChatGPT AI model.

In order to create blog posts you have to:
create an account via register endpoint
login and copy the jwt token
use jwt token when creating post, or AI post in specific Auth-Token header


## Before running:

setup appsettings.json with database parameters. If you want, use your own chat gpt token or use the one already present
in the appsettings.json, but know that there's a limit to how many blog posts you can create with AI.
run Update-Database in package manager console

	