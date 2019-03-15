﻿using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

using System.Net;
//using System.Text;  // for class Encoding
using System.IO;    // for StreamReader

using System.Threading;

namespace TwitchAnimals {

   [StaticConstructorOnStartup]   
   static class HarmonyPatches {
      // static IList<string> namePool = new string[] { };
      static IList<string> namePool = null;//{ get; set; }
      static IList<string> newNamePool = null;

      static ThreadStart pullNamesJob = null;
      static Thread pullNamesThread = null;

      static readonly object stopLock = new object();
      static string apiNames = "";
      static string ApiNames {
         get {
            lock (stopLock) {
               return apiNames;
            }
         }
         set {
            lock (stopLock) {
               apiNames = value;
            }
         }
      }

      static int skip = 0;


      // this static constructor runs to create a HarmonyInstance and install a patch.
      static HarmonyPatches() {
         HarmonyInstance harmony = HarmonyInstance.Create("rimworld.twitchanimals");

         // find the FillTab method of the class RimWorld.ITab_Pawn_Character


         MethodInfo wildAnimalSpawnerTickMeth = AccessTools.Method(typeof(RimWorld.WildAnimalSpawner), "WildAnimalSpawnerTick");
         // find the static method to call before (i.e. Prefix) the targetmethod
         HarmonyMethod wildAnimalSpawnerTickMethPrefix = new HarmonyMethod(typeof(TwitchAnimals.HarmonyPatches).GetMethod("WildAnimalSpawnerTick_Prefix"));
         // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
         harmony.Patch(wildAnimalSpawnerTickMeth, wildAnimalSpawnerTickMethPrefix, null);

         //WORKING overload pawn creation
         /*MethodBase pawnConstructorMeth = AccessTools.Constructor(typeof(Verse.Pawn));
         HarmonyMethod pawnConstructorMethPostfix = new HarmonyMethod(typeof(TwitchAnimals.HarmonyPatches).GetMethod("Pawn_Postfix"));
         HarmonyMethod pawnConstructorMethPrefix = new HarmonyMethod(typeof(TwitchAnimals.HarmonyPatches).GetMethod("Pawn_Prefix"));
         harmony.Patch(pawnConstructorMeth, pawnConstructorMethPrefix, pawnConstructorMethPostfix);
         */

         

         //overlaod name triple creation
         //TODO: inline array 
         //overload constructor with specific parameters (3x string)
         /*MethodBase nameConstructorMeth = AccessTools.Constructor(typeof(Verse.NameTriple), new System.Type[]{ typeof(string), typeof(string), typeof(string) });
         if (nameConstructorMeth == null) {
            ///TODO: why is this neccessary?
            return;
         }
         HarmonyMethod nameConstructorMethPostfix = new HarmonyMethod(typeof(TwitchAnimals.HarmonyPatches).GetMethod("NameTriple_Postfix"));
         harmony.Patch(nameConstructorMeth, null, nameConstructorMethPostfix);*/


         //overlaod name bank
         //TODO: inline array 
         //overload constructor with specific parameters (3x string)
         /*MethodInfo nameBankMeth = AccessTools.Method(typeof(RimWorld.NameBank), "GetName");
         if (nameBankMeth == null) {
            return;
         }
         HarmonyMethod nameBankMethPostfix = new HarmonyMethod(typeof(TwitchAnimals.HarmonyPatches).GetMethod("NameBank_Postfix"));
         harmony.Patch(nameBankMeth, null, nameBankMethPostfix);*/

      }

      /*public static void NameBank_Postfix(RimWorld.NameBank __instance, PawnNameSlot slot, Gender gender, bool checkIfAlreadyUsed, ref string __result) {
         Verse.Log.Message("NameBank_Postfix " + __result);
         if(slot == PawnNameSlot.First) {
            __result = "Twitch";
            return;
         }
         if(slot != PawnNameSlot.Nick) {
            return;
         }


         if(__result != null) {
            Verse.Log.Message(__result);
         }
         try {
            if (__instance == null) {
               return;
            }
            Verse.Log.Message(__instance.ToString());

            string name = getNameFromList();
            if (name == null || name.Length == 0) {
               Verse.Log.Message("DIRECT NAMING - NO NAMES");
               return;
            }

            Verse.Log.Message("DIRECT NAMING - GIVE: " + name);
            __result = name;
         }
         catch (System.Exception e) {
            Verse.Log.Message("DIRECT NAMING EXCEPTION " + e.ToString());
         }
      }*/

   

      /*public static void NameTriple_Postfix(Verse.NameTriple __instance, string first, string nick, string last/*, ref Pawn __result*) {
         if (__instance == null) {
            Verse.Log.Message("NameTriple_Postfix no instance");
            return;
         }
         if(first != null && first == "Twitch") {
            Verse.Log.Message("ALREADY Twitch");
            return;
         }
         Verse.Log.Message("NameTriple_Postfix");
         try {
            if (__instance == null) {
               return;
            }
            Verse.Log.Message(__instance.ToString());

            string name = getNameFromList();
            if (name == null || name.Length == 0) {
               Verse.Log.Message("DIRECT NAMING - NO NAMES");
               return;
            }

            Verse.Log.Message("2");

            Faction local_faction = localFaction();

            Verse.Log.Message("3");
            if (local_faction == null) {
               Verse.Log.Message("DIRECT NAMING - FACTION NULL");
               return;
            }
            Verse.Log.Message("4");

            Verse.NameTriple nametriple = new Verse.NameTriple("Twitch", name, "#" + name);
            Verse.Log.Message("DIRECT NAMING - GIVE: " + nametriple.ToString());
            __instance = nametriple;
         }
         catch (System.Exception e) {
            Verse.Log.Message("DIRECT NAMING EXCEPTION " + e.ToString());
         }
      }*/


      /*public static void Pawn_Prefix() {
         Verse.Log.Message("Pawn_Prefix");
      }

      public static void Pawn_Postfix(Pawn __instance/*, ref Pawn __result*) {
         if(__instance == null) {
            Verse.Log.Message("Pawn_Postfix no instance");
            return;
         }
         Verse.Log.Message("Pawn_Postfix");
         try {
            if (__instance.Name != null) {
               Verse.Log.Message(__instance.Name.ToString());
            }
            Verse.Log.Message("1");

            string name = getNameFromList();
            if(name == null || name.Length == 0) {
               Verse.Log.Message("DIRECT NAMING - NO NAMES");
               return;
            }

            Verse.Log.Message("2");

            Faction local_faction = localFaction();

            Verse.Log.Message("3");
            if (local_faction == null) {
               Verse.Log.Message("DIRECT NAMING - FACTION NULL");
               return;
            }
            Verse.Log.Message("4");

            Verse.NameTriple nametriple = new Verse.NameTriple("Twitch", name, "#" + name);
            Verse.Log.Message("DIRECT NAMING - GIVE: " + nametriple.ToString());
            __instance.Name = nametriple;
         }
         catch (System.Exception e) {
            Verse.Log.Message("DIRECT NAMING EXCEPTION " + e.ToString());
         }
      }*/


      static private void checkApiNames() {
         //Verse.Log.Message("checkApiNames");
         //Verse.Log.Message(System.Diagnostics.Process.GetCurrentProcess().Threads.Count);

         if (pullNamesJob == null) {
            //Verse.Log.Message("created initial pullNamesJob");
            pullNamesJob = new ThreadStart(pullNames);
         }

         if (pullNamesThread == null || pullNamesThread.ThreadState == ThreadState.Stopped) {
            //Verse.Log.Message("created new thread");
            pullNamesThread = new Thread(pullNamesJob);
         }

         if (pullNamesThread != null && !pullNamesThread.IsAlive) {
            //Verse.Log.Message("starting thread");
            pullNamesThread.Start();
         }

         if (ApiNames != "") {
            parseNames();
            ApiNames = "";
         }
      }

      static private string getNameFromList() {
         if (namePool == null) {
            return "";
         }
         if (namePool.Count() == 0) {
            return "";
         }
         string name = namePool.RandomElement();
         namePool.Remove(name);
         return name;
      }

      static private bool nameAlreadyExists(string name) {


         //Verse.Log.Message("CHECK FOR " + name);
         List<Pawn> pawns = Verse.Find.CurrentMap.mapPawns.AllPawnsSpawned;
         for (int j = 0; j < pawns.Count(); j++) {
            if (pawns.ElementAt(j).Name != null) {
               //Verse.Log.Message(" -> " + pawns.ElementAt(j).Name.ToString());
               if (pawns.ElementAt(j).AnimalOrWildMan() && pawns.ElementAt(j).Name.ToString() == name) {
                  //Verse.Log.Message("EXISTS");
                  return true;
               }
               try {
                  if (((Verse.NameTriple)pawns.ElementAt(j).Name).Nick == name) {
                     //Verse.Log.Message("EXISTS (NO ANIMAL)");
                     return true;
                  }
               }
               catch (System.Exception) {
                  //Verse.Log.Message("RENAME EXCEPTION " + e.ToString());
               }
            }
         }
         return false;
      }

      static private void parseNames() {
         //TODO: verify that this actually creates a copy
         string copiedNames = ApiNames;
         //Verse.Log.Message("NEW NAMES LIST");
         int count = 0;


         if (newNamePool == null) {
            newNamePool  = new List<string>();
         }
         newNamePool.Clear();

         using (StringReader reader = new StringReader(copiedNames)) {
            
            
            string line;
            //names = Enumerable.Repeat("", 0);
            while ((line = reader.ReadLine()) != null) {
               //TODO: USE NAME
               //Verse.Log.Message(line);
               count++;

               //Verse.Log.Message("ADDED " + line);
               if (!nameAlreadyExists(line)) {

                  //Verse.Log.Message("ADD UNIQUE " + line);
                  newNamePool.Add(line);
               }
            }
         }

         namePool = newNamePool;

         //Verse.Log.Message("LOADED NAMES: " + namePool.Count().ToString() + "/" + count.ToString());         
      }

      static private void pullNames() {
         
         var request = (HttpWebRequest)WebRequest.Create("http://rimworld.me/bonjwa/");
         var response = (HttpWebResponse)request.GetResponse();
         var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

         ApiNames = responseString;

         //TODO: remove
         //artificial slow-ness for testing
         //Thread.Sleep(300);

         //Verse.Log.Message("PULLED NAMES");
      }

      static private void nameAnimals() {


         //get all pawns on the map that are wild animals
         IEnumerable<Pawn> pawns = from p in Verse.Find.CurrentMap.mapPawns.AllPawnsSpawned
                                   where p.RaceProps.Animal &&
                                         !Find.CurrentMap.fogGrid.IsFogged(p.Position) &&
                                         p.Faction == null || p.Faction == Faction.OfInsects || p.Faction == Faction.OfMechanoids &&
                                         (p.mindState.Active || p.Dead)
                                   orderby (p.Name != null && !p.Name.Numerical) ? p.Name.ToStringShort : p.Label
                                   select p;

         //construct a name
         //RimWorld.JobGiver_Nuzzle jobg = new RimWorld.JobGiver_Nuzzle();
         //jobg.
         //Verse.JobDef
         //Job job = new Job(DefDatabase<JobDef>.GetNamed("GotoCellAndDraft"), thing);
         //workPawn.playerController.TakeOrderedJob(job);


         //give name to all
         //Pawn wild_animal = null;
         for (int i = 0; i < pawns.Count(); i++) {
            try {
               if (pawns.ElementAt(i).Name == null) {
                  string one_name_str = getNameFromList();

                  if (one_name_str == null || one_name_str.Length == 0) {
                     Verse.Log.Message("NO NAMES FOR ANIMALS");
                     return;
                  }

                  Verse.NameSingle name = new Verse.NameSingle(one_name_str);

                  Verse.Log.Message("GIVE NAME (ANIMAL): " + one_name_str);
                  //Verse.Log.Message("named a schmatteo at " + i.ToString());
                  pawns.ElementAt(i).Name = name;

                  //TODO: write to list to make api call and notify server about new animals

               }
               //else {
               //   wild_animal = pawns.ElementAt(i);
               //}
            }
            catch (System.Exception e) {
               Verse.Log.Message("RENAME EXCEPTION " + e.ToString());
            }
         }
      } // NAME ANIMALS END


      static private Faction localFaction() {
         Faction local_faction = null;
         Pawn colonist = null;

         IEnumerable<Pawn> colonist_pawns = from p in Verse.Find.CurrentMap.mapPawns.FreeColonistsAndPrisoners
                                            where !p.RaceProps.Animal && p.Name != null && !p.Name.Numerical
                                            select p;

         //find local faction
         for (int i = 0; i < colonist_pawns.Count(); i++) {
            //Verse.Log.Message("COLONIST: " + colonist_pawns.ElementAt(i).Name);
            //info = colonist_pawns.ElementAt(i).attack()
            colonist = colonist_pawns.ElementAt(i);

            local_faction = colonist.Faction;
            if (local_faction != null) {
               //Verse.Log.Message("FACTION " + local_faction.ToString());
               break;
            }
         }

         return local_faction;
      }

      static private void nameOther() {
         Faction local_faction = localFaction();

         if(local_faction == null) {
            Verse.Log.Message("FACTION null");
            return;
         }

         
         IEnumerable<Pawn> others_pawns = from p in Verse.Find.CurrentMap.mapPawns.AllPawnsSpawned
                                            where !p.RaceProps.Animal && p.Name != null && !p.Name.Numerical
                                            select p;
         for (int i = 0; i < others_pawns.Count(); i++) {
            try {
               if (others_pawns.ElementAt(i).Faction != local_faction
                  && ((Verse.NameTriple)others_pawns.ElementAt(i).Name).First != "Twitch") {

                  string name = getNameFromList();
                  if (name == null || name.Length == 0) {
                     Verse.Log.Message("NO NAMES FOR OTHERS");
                     return;
                  }

                  Verse.NameTriple nametriple = new Verse.NameTriple("Twitch", name, "#" + name);
                  Verse.Log.Message("GIVE NAME (OTHER): " + nametriple.ToString());
                  others_pawns.ElementAt(i).Name = nametriple;
                  //Verse.Log.Message("TITLE: " + others_pawns.ElementAt(i).story.adul);
               }
            }
            catch (System.Exception e) {
               Verse.Log.Message("RENAME EXCEPTION " + e.ToString());
            }
            
         }

         //rename other pawns
         /*for (int i = 0; i < others_pawns.Count(); i++) {

            try {
               if (others_pawns.ElementAt(i).Faction != local_faction
               && ((Verse.NameTriple)others_pawns.ElementAt(i).Name).First != "Twitch") {

                  string one_name_str = "";
                  bool found_name = false;
                  for (int f = 0; f < namePool.Count(); f++) {
                     one_name_str = namePool.ElementAt(f);

                     if (usedOtherNames.IndexOf(one_name_str) == -1) {
                        found_name = true;
                        break;
                     }
                  }

                  if (!found_name) {
                     Verse.Log.Message("NO UNIQUE NAME FOR OTHERS");
                     break;
                  }
                  usedOtherNames.Add(one_name_str);

                  string last = "";
                  /*if (one_name_str == "BonjwaRedpanda") {
                     others_pawns.ElementAt(i).skills.Learn(RimWorld.SkillDefOf.Shooting, 999999);
                     last = "The Shooter";

                     others_pawns.ElementAt(i).story.traits.allTraits.Clear();

                     RimWorld.Trait shooter = new RimWorld.Trait(RimWorld.TraitDefOf.ShootingAccuracy, 1);
                     others_pawns.ElementAt(i).story.traits.GainTrait(shooter);
                     
                     RimWorld.Trait bloodlust = new RimWorld.Trait(RimWorld.TraitDefOf.Bloodlust);
                     others_pawns.ElementAt(i).story.traits.GainTrait(bloodlust);

                     RimWorld.Trait beauty = new RimWorld.Trait(RimWorld.TraitDefOf.Beauty, 1);
                     others_pawns.ElementAt(i).story.traits.GainTrait(beauty);

                     RimWorld.Trait sens = new RimWorld.Trait(RimWorld.TraitDefOf.PsychicSensitivity, 1);
                     others_pawns.ElementAt(i).story.traits.GainTrait(sens);
                  }

                  if (one_name_str == "Moobot") {
                     others_pawns.ElementAt(i).skills.Learn(RimWorld.SkillDefOf.Melee, 999999);
                     last = "will kill you with bare hands";

                     //strip to drop all weapons and meele fight
                     others_pawns.ElementAt(i).Strip();

                     //give traits
                     others_pawns.ElementAt(i).story.traits.allTraits.Clear();

                     RimWorld.Trait bloodlust = new RimWorld.Trait(RimWorld.TraitDefOf.Bloodlust);
                     others_pawns.ElementAt(i).story.traits.GainTrait(bloodlust);

                     RimWorld.Trait tough = new RimWorld.Trait(RimWorld.TraitDefOf.Tough);
                     others_pawns.ElementAt(i).story.traits.GainTrait(tough);

                     RimWorld.Trait brawler = new RimWorld.Trait(RimWorld.TraitDefOf.Brawler);
                     others_pawns.ElementAt(i).story.traits.GainTrait(brawler);

                     RimWorld.Trait sens = new RimWorld.Trait(RimWorld.TraitDefOf.PsychicSensitivity, 1);
                     others_pawns.ElementAt(i).story.traits.GainTrait(sens);
                  }

                  if (one_name_str == "Kerrag") {
                     others_pawns.ElementAt(i).skills.Learn(RimWorld.SkillDefOf.Cooking, 999999);
                     last = "The Cook";


                     //give traits
                     others_pawns.ElementAt(i).story.traits.allTraits.Clear();

                     RimWorld.Trait sensitive = new RimWorld.Trait(RimWorld.TraitDefOf.PsychicSensitivity, 1);
                     others_pawns.ElementAt(i).story.traits.GainTrait(sensitive);

                     RimWorld.Trait trans = new RimWorld.Trait(RimWorld.TraitDefOf.Transhumanist);
                     others_pawns.ElementAt(i).story.traits.GainTrait(trans);

                     RimWorld.Trait indu = new RimWorld.Trait(RimWorld.TraitDefOf.Industriousness, 1);
                     others_pawns.ElementAt(i).story.traits.GainTrait(indu);
                  }
                  

                  if (one_name_str == "bonjwahonor") {
                     others_pawns.ElementAt(i).skills.Learn(RimWorld.SkillDefOf.Social, 999999);
                     last = "The Nice Guy";

                     //give traits
                     others_pawns.ElementAt(i).story.traits.allTraits.Clear();

                     RimWorld.Trait greed = new RimWorld.Trait(RimWorld.TraitDefOf.Greedy);
                     others_pawns.ElementAt(i).story.traits.GainTrait(greed);

                     RimWorld.Trait memory = new RimWorld.Trait(RimWorld.TraitDefOf.GreatMemory);
                     others_pawns.ElementAt(i).story.traits.GainTrait(memory);

                     RimWorld.Trait kind = new RimWorld.Trait(RimWorld.TraitDefOf.Kind);
                     others_pawns.ElementAt(i).story.traits.GainTrait(kind);

                     RimWorld.Trait pyro = new RimWorld.Trait(RimWorld.TraitDefOf.Pyromaniac);
                     others_pawns.ElementAt(i).story.traits.GainTrait(pyro);
                  }

                  if (one_name_str == "Neandi") {
                     others_pawns.ElementAt(i).skills.Learn(RimWorld.SkillDefOf.Artistic, 999999);
                     last = "The Artist";

                     //give traits
                     others_pawns.ElementAt(i).story.traits.allTraits.Clear();

                     RimWorld.Trait drug = new RimWorld.Trait(RimWorld.TraitDefOf.DrugDesire, 1);
                     others_pawns.ElementAt(i).story.traits.GainTrait(drug);

                     RimWorld.Trait ascet = new RimWorld.Trait(RimWorld.TraitDefOf.Ascetic);
                     others_pawns.ElementAt(i).story.traits.GainTrait(ascet);

                     RimWorld.Trait nudist = new RimWorld.Trait(RimWorld.TraitDefOf.Nudist);
                     others_pawns.ElementAt(i).story.traits.GainTrait(nudist);

                     RimWorld.Trait psycho = new RimWorld.Trait(RimWorld.TraitDefOf.Psychopath);
                     others_pawns.ElementAt(i).story.traits.GainTrait(psycho);
                  }*


                  Verse.Log.Message("GIVE NAME (OTHER): " + one_name_str);
                  Verse.NameTriple nametriple = new Verse.NameTriple("Twitch", one_name_str, last);
                  others_pawns.ElementAt(i).Name = nametriple;
               }
            }
            catch (System.Exception) {
               Verse.Log.Message("RENAME EXCEPTION ");
            }
         }*/
      }



      public static void WildAnimalSpawnerTick_Prefix() {

         skip++;
         skip = skip % 200;

         if (skip != 0) {
            return;
         }

         checkApiNames();

         //Verse.Log.Message("WildAnimalSpawnerTick_Prefix");

         if (namePool == null || namePool.Count() == 0) {
            Verse.Log.Message("NO NAMES IN POOL");
            return;
         }

         nameAnimals();
         nameOther();


         /*Pawn colonist = null;
         Faction local_faction = null;
         IEnumerable<Pawn> colonist_pawns = from p in Verse.Find.CurrentMap.mapPawns.FreeColonistsAndPrisoners
                                            where p.Name != null && !p.Name.Numerical
                                            select p;
         for (int i = 0; i < colonist_pawns.Count(); i++) {
            //Verse.Log.Message("COLONIST: " + colonist_pawns.ElementAt(i).Name);
            //info = colonist_pawns.ElementAt(i).attack()
            colonist = colonist_pawns.ElementAt(i);

            local_faction = colonist.Faction;
            if (local_faction != null) {
               //Verse.Log.Message("FACTION " + local_faction.ToString());
            }
         }

         if(local_faction == null) {
            Verse.Log.Message("FACTION null");
         }*/


         //if (names == null) {
         //Verse.Log.Message("created original schmatteo");
         //names = Enumerable.Repeat("Schmatteo", 1);

         //var request = (HttpWebRequest)WebRequest.Create("http://tmi.twitch.tv/group/user/bonjwa/chatters");

         //var response = (HttpWebResponse)request.GetResponse();

         //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

         //TODO: implement parser

         //Verse.Log.Message(responseString);
         //}

         /*if(wild_animal != null) {
            Verse.Log.Message("WILD ANIMAL: " + wild_animal.Name.ToString());
            if (local_faction != null) {
               wild_animal.SetFaction(local_faction);
               wild_animal.TryStartAttack(colonist);
            }
            wild_animal.training.Train(RimWorld.TrainableDefOf.Tameness, colonist, true);
            wild_animal.training.Train(RimWorld.TrainableDefOf.Obedience, colonist, true);
            wild_animal.training.Train(RimWorld.TrainableDefOf.Release, colonist, true);

            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Tameness);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Obedience);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Release);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Tameness);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Obedience);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Release);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Tameness);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Obedience);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Release);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Tameness);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Obedience);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Release);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Tameness);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Obedience);
            wild_animal.training.HasLearned(RimWorld.TrainableDefOf.Release);
         }*/

         /*if (colonist != null) {
            List<Verse.AI.ThinkNode> nodes = colonist.thinker.MainThinkNodeRoot.subNodes;
            for (int j = 0; j < nodes.Count(); j++) {
               Verse.Log.Message("thinker " + nodes.ElementAt(j).ToString());

               if (nodes.ElementAt(j).ToString() == "RimWorld.JobGiver_Nuzzle") {
                  if (colonist != null) {
                     Verse.Log.Message("Nuzzle " + colonist.Name.ToString());
                     Verse.AI.JobIssueParams paramss = new Verse.AI.JobIssueParams();
                     paramss.maxDistToSquadFlag = 99999;
                     Verse.AI.ThinkResult res = nodes.ElementAt(j).TryIssueJobPackage(colonist, paramss);

                     if (!res.IsValid) {
                        Verse.Log.Message("not valid");
                     }
                  }
               }
            }
         }*/


         /*pawns = from p in Verse.Find.CurrentMap.mapPawns.AllPawnsSpawned
                                   where p.RaceProps.Animal
                                   select p;

         Pawn lucie = null;

         for (int i = 0; i < pawns.Count(); i++) {
            if (pawns.ElementAt(i).Name.ToString() == "Lucie") {
               lucie = pawns.ElementAt(i);
               Verse.Log.Message("LUCIE");

               //RimWorld.JobGiver_Nuzzle nuzz = new RimWorld.JobGiver_Nuzzle();
               //pawns.ElementAt(i).thinker.MainThinkNodeRoot.subNodes.Add(nuzz);
               //pawns.ElementAt(i).thinker.ConstantThinkNodeRoot.subNodes.Add(nuzz);
               //pawns.ElementAt(i).jobs.StartJob()

            }
         }*/




         /*try {
            if (i + 1 < pawns.Count()) {
               string target_name = pawns.ElementAt(i + 1).Name.ToString();
               string attacker_name = pawns.ElementAt(i).Name.ToString();
               Verse.Log.Message("attack by " + target_name + " on " + attacker_name);
               LocalTargetInfo info = new LocalTargetInfo(pawns.ElementAt(i + 1));
               if (!pawns.ElementAt(i).TryStartAttack(info)) {
                  Verse.Log.Message("attack failed");
               }
            }
         }
         catch (TargetException e) {
            Verse.Log.Message("ERR");
         }*/
         //}
         //Verse.AI.Pawn_Thinker
         //LocalTargetInfo info = null;
         //IEnumerable<Pawn> colonist_pawns = Verse.Find.CurrentMap.mapPawns.;




      }
   }
}















         //MapPawns pawns2 = Verse.Find.CurrentMap.mapPawns;
         //IEnumerable<Pawn> pawns = pawns2.AllPawns;





         //IEnumerable<Pawn> pawns = RimWorld.PawnsFinder.AllMaps.AsEnumerable<Pawn>();

         /*IEnumerable<string> names_str = Enumerable.Repeat("Schmateo", 1);


         RimWorld.NameBank bank = new RimWorld.NameBank(PawnNameCategory.NoName);

         bank.AddNames(PawnNameSlot.Only, Gender.None, names_str);

         IEnumerable<NameTriple> names = RimWorld.PawnNameDatabaseSolid.AllNames();
         NameTriple one_name = names.RandomElement<NameTriple>();*/
         //Pawn one = RimWorld.PawnsFinder.Temporary.RandomElement<Pawn>();
         //Pawn one = RimWorld.PawnsFinder.AllMaps_Spawned.RandomElement<Pawn>();
         
         //RimWorld.PawnUtility.GiveNameBecauseOfNuzzle()
         //RimWorld.PawnTable_Wildlife a ;
         //Rimworld
         //Verse.Log.Message(a.ToString());
         //Log.Warning("hello world");