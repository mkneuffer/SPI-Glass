This is the dialogue for the sword cutscenes

Look, over there! The mighty Excalibur! #speaker:Archibald #layout:left
Can you try pulling it out? It'd make our job a hundred times simpler. #speaker:Archibald #layout:left
-> sword1

===sword1===
    * [I'll try pulling it...] It's not budging at all!
        Are you sure you're pulling it hard enough? Hmmm, maybe the sword is sealed for
        now. No matter, it's best that we keep moving! No doubt that ghost will try
        claiming the sword for himself. #speaker:Archibald #layout:left
        -> DONE
    + [Why do we need Excalibur?]
        It's a weapon, of course we need it! #speaker:Archibald #layout:left
        -> sword1
    + [What if I refuse?]
        Did you forget that there's a crazy ghost after both of us?! Pull it out or
        -> sword1
        
Next scene
-> sword2
===sword2===
Throw the grail! #speaker:Archibald #layout:left
    * [OK!]
        Alright, now go pull the sword out while he's distracted
        -> DONE
    * [Repeat that for me?]
        -> sword2
-> END