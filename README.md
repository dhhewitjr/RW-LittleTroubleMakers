# RW-LittleTroubleMakers
## RimWorld 1.4 to let the Children from the BioTech DLC get into more trouble when doing learning activities.

Teemo my Beemo

---
## Code Format Rules
### Preamble
It is preferred than any contributers (dhhewitjr / Ageond maybe) follow rules so we stay consistent as to avoid upsetting SageTheWizard
Any wanders joining this, please also follow the rules but I trust if you get involved you have some knowledge of RimWorld Modding / C# atleast...
### Variable Names
Please follow [Hungarian Notation](https://en.wikipedia.org/wiki/Hungarian_notation) and have DESCRIPTIVE Variable names
### Avoid "var"
Please use explicit types unless it is some really awful class `SomeClass.SubClass.Thing.Ahhh<LongClass.Stuff>` can be shortened with `var` but for shorter class names, please avoid the use of `var`
### No Breaks / Continues
No using `break;` in loops, if you absolutely need a break for your loop, please reconsider your algorithm.  Similarly with continues, however there are more cases where I can see this being useful
### If / Else if / Else
Please don't have a > 3 nested ifs, reconsider the algorithm if this is happening... 

If it makes sense to do so, > 3 (one else if) conditional blocks, please use a switch statement instead.
### Never Ever Commit directly to Master 
Create a branch, work in branch, merge to master once repository is fully set up.
