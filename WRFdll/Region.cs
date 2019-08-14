﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRFdll
{
    internal static class Region
    {
        public static Dictionary<string, string> ORP = new Dictionary<string, string>
        {
            {"#006400","Český Těšín"},
            {"#d70000","Klatovy"},
            {"#00d700","Kralovice"},
            {"#c800c8","Kadaň"},
            {"#00c8c8","Litoměřice"},
            {"#00fa00","Dobříš"},
            {"#00c3c3","Roudnice nad Labem"},
            {"#d2d200","Aš"},
            {"#c8c800","Litvínov"},
            {"#f000f0","Rakovník"},
            {"#0000dc","Blovice"},
            {"#0000d7","Nepomuk"},
            {"#be00be","Žatec"},
            {"#00dcdc","Horažďovice"},
            {"#d20000","Rokycany"},
            {"#dc0000","Vimperk"},
            {"#0000c3","Most"},
            {"#c80000","Bílina"},
            {"#fafa00","Kralupy nad Vltavou"},
            {"#f5f500","Neratovice"},
            {"#0000be","Varnsdorf"},
            {"#00f500","Lysá nad Labem"},
            {"#afaf00","Nová Paka"},
            {"#00af00","Jaroměř"},
            {"#aa0000","Nové Město nad Metují"},
            {"#aaaa00","Česká Třebová"},
            {"#006e00","Otrokovice"},
            {"#007373","Holešov"},
            {"#005f5f","Kravaře"},
            {"#5f005f","Kopřivnice"},
            {"#646400","Havířov"},
            {"#005f00","Jablunkov"},
            {"#b40000","Turnov"},
            {"#00f000","Poděbrady"},
            {"#b90000","Jablonec nad Nisou"},
            {"#0000af","Jičín"},
            {"#009b9b","Humpolec"},
            {"#910000","Světlá nad Sázavou"},
            {"#00b400","Železný Brod"},
            {"#e60000","Dačice"},
            {"#b9b900","Tanvald"},
            {"#00b9b9","Semily"},
            {"#960000","Jihlava"},
            {"#009100","Telč"},
            {"#00ffff","Čáslav"},
            {"#af0000","Hradec Králové"},
            {"#a000a0","Přelouč"},
            {"#00b900","Jilemnice"},
            {"#00aa00","Nový Bydžov"},
            {"#9b009b","Havlíčkův Brod"},
            {"#005500","Vítkov"},
            {"#690069","Zlín"},
            {"#006e6e","Uherský Brod"},
            {"#00005a","Opava"},
            {"#00007d","Hranice"},
            {"#730073","Bystřice pod Hostýnem"},
            {"#005a00","Odry"},
            {"#6e0000","Luhačovice"},
            {"#006900","Vizovice"},
            {"#690000","Valašské Meziříčí"},
            {"#000069","Vsetín"},
            {"#5a0000","Nový Jičín"},
            {"#6e6e00","Valašské Klobouky"},
            {"#006969","Bílovec"},
            {"#005a5a","Ostrava"},
            {"#5f0000","Hlučín"},
            {"#00006e","Rožnov pod Radhoštěm"},
            {"#000064","Frenštát pod Radhoštěm"},
            {"#640064","Frýdek-Místek"},
            {"#006464","Frýdlant nad Ostravicí"},
            {"#00005f","Karviná"},
            {"#550000","Třinec"},
            {"#be0000","Teplice"},
            {"#e6e600","Prachatice"},
            {"#696900","Bohumín"},
            {"#c30000","Louny"},
            {"#00e100","Strakonice"},
            {"#00eb00","Vlašim"},
            {"#00f5f5","Mnichovo Hradiště"},
            {"#f00000","Nymburk"},
            {"#969600","Pelhřimov"},
            {"#00fafa","Kolín"},
            {"#009696","Pacov"},
            {"#f50000","Kutná Hora"},
            {"#bebe00","Frýdlant"},
            {"#b4b400","Hořice"},
            {"#009600","Moravské Budějovice"},
            {"#0000a5","Chrudim"},
            {"#9b9b00","Chotěboř"},
            {"#00a000","Pardubice"},
            {"#7d0000","Znojmo"},
            {"#00aaaa","Vrchlabí"},
            {"#00b4b4","Dvůr Králové nad Labem"},
            {"#000091","Třebíč"},
            {"#aa00aa","Trutnov"},
            {"#009191","Žďár nad Sázavou"},
            {"#a50000","Hlinsko"},
            {"#910091","Velké Meziříčí"},
            {"#00a500","Holice"},
            {"#00afaf","Náchod"},
            {"#b400b4","Dobruška"},
            {"#af00af","Kostelec nad Orlicí"},
            {"#960096","Nové Město na Moravě"},
            {"#9b0000","Vysoké Mýto"},
            {"#000096","Náměšť nad Oslavou"},
            {"#0000b4","Broumov"},
            {"#008787","Moravský Krumlov"},
            {"#0000a0","Polička"},
            {"#0000aa","Rychnov nad Kněžnou"},
            {"#a5a500","Litomyšl"},
            {"#00009b","Bystřice nad Pernštejnem"},
            {"#870000","Ivančice"},
            {"#820082","Tišnov"},
            {"#820000","Rosice"},
            {"#a0a000","Ústí nad Orlicí"},
            {"#009b00","Žamberk"},
            {"#00a0a0","Svitavy"},
            {"#eb00eb","Blatná"},
            {"#ebeb00","Český Krumlov"},
            {"#eb0000","Slaný"},
            {"#fa00fa","Kladno"},
            {"#00c300","Lovosice"},
            {"#00e6e6","Písek"},
            {"#0000f0","Příbram"},
            {"#0000fa","Hořovice"},
            {"#0000ff","Beroun"},
            {"#00be00","Ústí nad Labem"},
            {"#00dc00","Vodňany"},
            {"#ff0000","Hlavní město Praha"},
            {"#e1e100","Týn nad Vltavou"},
            {"#c3c300","Rumburk"},
            {"#0000e6","Kaplice"},
            {"#0000f5","Mělník"},
            {"#ff00ff","Brandýs nad Labem-Stará Boleslav"},
            {"#00bebe","Česká Lípa"},
            {"#0000e1","Tábor"},
            {"#00ff00","Benešov"},
            {"#8c0000","Boskovice"},
            {"#008700","Kuřim"},
            {"#878700","Pohořelice"},
            {"#000082","Šlapanice"},
            {"#008c00","Brno"},
            {"#870087","Mikulov"},
            {"#919100","Blansko"},
            {"#00a5a5","Lanškroun"},
            {"#007d00","Židlochovice"},
            {"#a00000","Moravská Třebová"},
            {"#a500a5","Králíky"},
            {"#8c8c00","Hustopeče"},
            {"#00008c","Břeclav"},
            {"#000073","Zábřeh"},
            {"#007800","Mohelnice"},
            {"#007d7d","Konice"},
            {"#730000","Šumperk"},
            {"#008200","Slavkov u Brna"},
            {"#780078","Prostějov"},
            {"#828200","Vyškov"},
            {"#780000","Litovel"},
            {"#7d007d","Jeseník"},
            {"#000087","Kyjov"},
            {"#008c8c","Hodonín"},
            {"#8c008c","Bučovice"},
            {"#007300","Uničov"},
            {"#000078","Olomouc"},
            {"#737300","Kroměříž"},
            {"#5a5a00","Rýmařov"},
            {"#787800","Šternberk"},
            {"#640000","Bruntál"},
            {"#007878","Přerov"},
            {"#6e006e","Uherské Hradiště"},
            {"#5f5f00","Krnov"},
            {"#008282","Veselí nad Moravou"},
            {"#e100e1","Trhové Sviny"},
            {"#b900b9","Nový Bor"},
            {"#0000eb","Votice"},
            {"#00f0f0","Říčany"},
            {"#e10000","Soběslav"},
            {"#00e1e1","Třeboň"},
            {"#0000b9","Liberec"},
            {"#f500f5","Mladá Boleslav"},
            {"#00e600","Jindřichův Hradec"},
            {"#fa0000","Český Brod"},
            {"#00d2d2","Tachov"},
            {"#0000cd","Kraslice"},
            {"#cd00cd","Mariánské Lázně"},
            {"#cdcd00","Sokolov"},
            {"#00cd00","Karlovy Vary"},
            {"#00cdcd","Ostrov"},
            {"#0000d2","Stříbro"},
            {"#cd0000","Cheb"},
            {"#dc00dc","Domažlice"},
            {"#dcdc00","Horšovský Týn"},
            {"#00d200","Stod"},
            {"#d700d7","Nýřany"},
            {"#7d7d00","Lipník nad Bečvou"},
            {"#ffff00","Černošice"},
            {"#f0f000","Sedlčany"},
            {"#e600e6","Milevsko"},
            {"#00c800","Děčín"},
            {"#00ebeb","České Budějovice"},
            {"#5a005a","Orlová"},
            {"#c300c3","Podbořany"},
            {"#d200d2","Sušice"},
            {"#00d7d7","Plzeň"},
            {"#d7d700","Přeštice"},
            {"#0000c8","Chomutov"}
        };
    }
}
