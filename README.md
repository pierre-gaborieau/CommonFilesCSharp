# CommonFilesCSharp
I will post here all the files that I commonly use in my projects
I advice you to create a project inside your solution with those file.
All those classes are static so you can simmplify your invocation like this :
> using TR = 'NameSpace'.Common.TestResult_Printer;

# TestResult_Printer.cs
This file is used to log the result of your UnitTests.
Admitting you called it TR in your imports here is how it works : 

#### For a single variable result
>TR.Print_singleResult(yourResult) 


# MappingModel.cs
