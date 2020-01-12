# SETrimToClipboard
A simple tool for adding a Space Engineers script to your clipboard for easy pasting in-game. It will also trim most of the whitespace and line breaks (to fit large scripts within the 100 000 character limit).

## How it works

1. It accepts a path to a folder. It will prompt you unless you run the program with the path as an argument.
2. It looks through each `.cs` file in it in alphabetical order (won't look in sub folders).
3. It grabs everything within specific regions and processes the code based on region.
4. It puts it in your clipboard. 

Then you can just paste it in-game. 

## Regions

```cs
#region untouched
	//Copied intact
#endregion

//Ignored

#region in-game
	// Copied but
	// - Comments removed
	// - Whitespace removed
	// - Line breaks removed*
#endregion
```

\* It keeps line breaks roughly every 200 character, as the game crashes on some computers when trying to display really long lines of text.
