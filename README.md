# Semantic Kernel Skills Application
Basic commandline tool which you cna use to run a Semantic Kernel semantic function or a LangChain prompt.

Built with .NET 7.0

Clone the repo
```powershell
git clone 
```

Build the project
```powershell
dotnet build
```

Configure Azure OpenAI
```powershell
$Env:AZURE_OPENAI_DEPLOYMENT_LABEL = "text-davinci-003"
$Env:AZURE_OPENAI_DEPLOYMENT_NAME = "text-davinci-003"
$Env:AZURE_OPENAI_API_ENDPOINT = "<Your API endpoint>"
$Env:AZURE_OPENAI_API_KEY = "<Your API key>"
```

Run a semantic function
```powershell
dotnet run -- runfunc -l C:\SemanticKernel\repos\semantic-kernel\samples\skills -s FunSkill -f Joke 
Generate a funny joke
Enter input > Dogs
Enter style > Seinfeld
Q: What did the dog say when he sat on the sandpaper?
A: Ruff!
```

Run a LangChain prompt
```powershell
dotnet run -- runprompt -l C:\\SemanticKernel\\repos\\langchain-hub\\prompts -p llm_math -f llm_math_prompt
Enter question > what is 12 * 12 * 12 * 12?
Answer: 20736
```
