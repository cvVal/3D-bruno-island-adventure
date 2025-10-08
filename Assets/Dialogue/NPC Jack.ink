EXTERNAL VerifyQuest()

VAR questCompleted = false
VAR hasMetJack = false
VAR hasAskedForDetails = false
VAR knowsAboutIsland = false

-> start

=== start ===
{
    - questCompleted:
        -> post_completion
    - hasMetJack:
        -> already_met
    - else:
        ~ hasMetJack = true
        -> first_meeting
}

=== first_meeting ===
Oh! A visitor!
I wasn't expecting anyone to come all the way out here.
Welcome, traveler. I'm Jack, keeper of this little beach shop.
Well... I was keeping it running smoothly, until recently.
You see, something terrible happened a few days ago.
Pirates landed on our shores under cover of fog.
They plundered the nearby homes, scattered supplies across the sand...
And they took something very precious from me.
+ [What did they take?]
    -> explain_candy
+ [Pirates? When did this happen?]
    ~ knowsAboutIsland = true
    -> pirate_raid
+ [I can help you.]
    You would? Oh, that's... that's very kind of you!
    -> explain_candy

=== pirate_raid ===
Three nights ago, when the tide was high and the moon was hidden.
They sailed in from the eastern waters, silent as shadows.
By the time we heard them, they'd already ransacked half the island.
A few of us tried to fight back, but...
They were experienced cutthroats. We didn't stand a chance.
They took what they wanted and vanished into the mist.
+ [What did they steal?]
    -> explain_candy
+ [I'll help get it back.]
    Truly? You're braver than you look, friend.
    -> explain_candy

=== explain_candy ===
They took my Lunar Bonbon.
I know, I know—it sounds silly.
"It's just candy," you're thinking.
But this wasn't ordinary candy.
It was wrapped in sunset-orange silk, tied with a silver thread.
At dusk, a tiny rune glows softly on its surface—my own enchantment.
I crafted it during the last harvest moon...
Using morning dew from island ferns, crystalline sap from a traveling merchant.
They claimed the sap fell from a comet's tail.
I don't know if that's true, but...
The candy has always felt special to me.
It holds memories I can't replace.
+ [Why is it so important?]
    -> candy_meaning
+ [Where did they hide it?]
    ~ knowsAboutIsland = true
    -> search_location
+ [I'll find it for you.]
    Your kindness gives me hope, traveler.
    May the sea breeze guide you safely.
    -> ask_if_found

=== candy_meaning ===
My late grandmother taught me the recipe.
She used to say that sweets aren't just for the tongue—
They're for the heart, the soul, the memories we hold dear.
That Lunar Bonbon... it's the last one I made with her teachings.
The last piece of her still with me.
When those pirates took it, they didn't just steal candy.
They stole a part of my past.
+ [I'll get it back. Promise.]
    Thank you, friend. Truly.
    You've no idea what this means to me.
    -> search_location
+ [Where should I look?]
    ~ knowsAboutIsland = true
    -> search_location

=== search_location ===
The pirates left in a hurry—they couldn't carry everything.
The chest should be somewhere around the upper northwestern area.
I'd go myself, but...
There might still be pirates lurking about, keeping watch.
Please, be careful out there.
+ [I'll be careful.]
    I believe in you, friend.
    Return safely.
    -> ask_if_found
+ { not hasAskedForDetails } [Describe the candy]
    ~ hasAskedForDetails = true
    -> candy_description

=== candy_description ===
Look for sunset-orange silk, tied with silver thread.
The candy itself is about the size of a walnut.
When dusk falls, you'll see a faint amber glow—
That's the rune I etched into its surface.
You'll know it when you see it, I promise.
+ [Got it. I'll search now.]
    Safe travels, friend.
    May the tides be in your favor.
    -> ask_if_found
+ [Where are the cliffs?]
    -> search_reminder

=== already_met ===
Ah, you've returned!
The sea brings you back to me, I see.
-> ask_if_found

=== ask_if_found ===
Have you found my Lunar Bonbon in your travels?
+ [Yes! Here it is.]
    ~ VerifyQuest()
    {
        - questCompleted:
            -> success
        - else:
            -> wrong_item
    }
+ [Still looking for it.]
    -> still_searching
+ [Remind me where?]
    -> search_reminder
+ { hasAskedForDetails == false } [What's it look like?]
    ~ hasAskedForDetails = true
    -> candy_description

=== still_searching ===
I understand. These things take time.
The northern cliffs can be tricky to navigate.
Just... please be safe out there.
And if you spot anything wrapped in orange silk with an amber glow...
That's the one.
Bring it back to me when you find it.
May fortune smile upon you, traveler.
-> END

=== search_reminder ===
The pirates buried it in the upper northwestern area.
Head north, you'll pass the bridge on your way.
Keep going until you reach the rocky statue.
Return when you've recovered my treasure, friend.
-> END

=== wrong_item ===
Hmm... I'm afraid that's not quite it.
My Lunar Bonbon has sunset-orange silk and a glowing rune.
Perhaps you found something else?
An honest mistake—there's likely all sorts of loot in that chest.
Keep searching near the upper northwestern area.
You'll recognize the real one when you see it.
Trust me.
-> END

=== success ===
...!
You... you actually found it!
The ribbon... the rune... it's perfect! Still intact!
I'd almost given up hope.
Here, please—take this as thanks.
Jack hands you a sword 🗡️ and 2 potions 🧪! You feel stronger already!
It's not much, but it's all I can offer.
You've given me more than just candy back.
You've restored a piece of my heart, my memories.
My grandmother would have been proud to know there are still kind souls like you.
Remember, traveler:
Kindness echoes far louder than the sharpest blade.
May your journey across these islands be blessed with good fortune.
~ questCompleted = true
-> END

=== post_completion ===
Welcome back, friend!
My Lunar Bonbon sits safely on the shelf now.
Each evening, when it glows softly, I'm reminded...
Reminded that good deeds ripple outward like waves upon the shore.
You've done more for this old shopkeeper than you know.
If you ever need a sweet treat or a word of encouragement...
You know where to find me.
Safe travels, hero.
-> END
