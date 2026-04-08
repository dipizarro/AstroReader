using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations;

internal static class InterpretationCatalog
{
    public static readonly IReadOnlyDictionary<ZodiacSign, string> SunBySign = new Dictionary<ZodiacSign, string>
    {
        [ZodiacSign.Aries] = "Tu Sol en Aries te dota de iniciativa, impulso y deseo de abrir camino con valentia.",
        [ZodiacSign.Taurus] = "Tu Sol en Tauro expresa constancia, sentido practico y una identidad que busca estabilidad real.",
        [ZodiacSign.Gemini] = "Tu Sol en Geminis resalta curiosidad, versatilidad y una identidad que se fortalece explorando ideas.",
        [ZodiacSign.Cancer] = "Tu Sol en Cancer pone en el centro la sensibilidad, la proteccion y el valor de los vinculos cercanos.",
        [ZodiacSign.Leo] = "Tu Sol en Leo busca crear, brillar y expresarse con generosidad, orgullo sano y presencia personal.",
        [ZodiacSign.Virgo] = "Tu Sol en Virgo se orienta al detalle, la mejora continua y la necesidad de ser util de manera concreta.",
        [ZodiacSign.Libra] = "Tu Sol en Libra necesita equilibrio, cooperacion y belleza para afirmar su identidad.",
        [ZodiacSign.Scorpio] = "Tu Sol en Escorpio vive la identidad con intensidad, profundidad y deseo de transformacion verdadera.",
        [ZodiacSign.Sagittarius] = "Tu Sol en Sagitario crece buscando sentido, expansion, vision y libertad para explorar.",
        [ZodiacSign.Capricorn] = "Tu Sol en Capricornio construye identidad a traves de disciplina, ambicion y logros sostenidos.",
        [ZodiacSign.Aquarius] = "Tu Sol en Acuario se afirma a traves de originalidad, independencia y mirada colectiva.",
        [ZodiacSign.Pisces] = "Tu Sol en Piscis conecta identidad con empatia, imaginacion y una percepcion muy sutil del entorno."
    };

    public static readonly IReadOnlyDictionary<ZodiacSign, string> MoonBySign = new Dictionary<ZodiacSign, string>
    {
        [ZodiacSign.Aries] = "Emocionalmente reaccionas rapido y necesitas movimiento, iniciativa y autonomia para sentirte vivo.",
        [ZodiacSign.Taurus] = "Tu Luna en Tauro busca calma, contencion y seguridad tangible para regular lo que sientes.",
        [ZodiacSign.Gemini] = "Tu Luna en Geminis procesa emociones conversando, nombrando y entendiendo mentalmente lo que pasa.",
        [ZodiacSign.Cancer] = "Tu Luna en Cancer siente profundamente, protege lo amado y necesita refugio afectivo.",
        [ZodiacSign.Leo] = "Tu Luna en Leo necesita calor, reconocimiento y una expresion emocional noble y visible.",
        [ZodiacSign.Virgo] = "Tu Luna en Virgo ordena las emociones a traves de la observacion, el cuidado practico y el detalle.",
        [ZodiacSign.Libra] = "Tu Luna en Libra busca armonia y reciprocidad; el conflicto sostenido te desgasta con facilidad.",
        [ZodiacSign.Scorpio] = "Tu Luna en Escorpio vive emociones intensas, intuitivas y transformadoras, con gran profundidad vincular.",
        [ZodiacSign.Sagittarius] = "Tu Luna en Sagitario necesita aire, horizonte y libertad para no sentirse atrapada.",
        [ZodiacSign.Capricorn] = "Tu Luna en Capricornio contiene, administra y madura lo emocional con seriedad.",
        [ZodiacSign.Aquarius] = "Tu Luna en Acuario filtra lo emocional con distancia, ideas y necesidad de espacio personal.",
        [ZodiacSign.Pisces] = "Tu Luna en Piscis absorbe mucho del ambiente y necesita sensibilidad, arte y descanso emocional."
    };

    public static readonly IReadOnlyDictionary<ZodiacSign, string> AscendantBySign = new Dictionary<ZodiacSign, string>
    {
        [ZodiacSign.Aries] = "Tu Ascendente Aries te hace ver directo, rapido y dispuesto a avanzar sin demasiadas vueltas.",
        [ZodiacSign.Taurus] = "Tu Ascendente Tauro proyecta serenidad, consistencia y una presencia firme.",
        [ZodiacSign.Gemini] = "Tu Ascendente Geminis muestra curiosidad, agilidad y una forma vivaz de entrar en contacto.",
        [ZodiacSign.Cancer] = "Tu Ascendente Cancer se percibe sensible, protectivo y receptivo desde el primer encuentro.",
        [ZodiacSign.Leo] = "Tu Ascendente Leo irradia presencia, confianza y una impronta expresiva dificil de ignorar.",
        [ZodiacSign.Virgo] = "Tu Ascendente Virgo proyecta observacion, criterio y una imagen sobria pero servicial.",
        [ZodiacSign.Libra] = "Tu Ascendente Libra transmite diplomacia, encanto y buena lectura del otro.",
        [ZodiacSign.Scorpio] = "Tu Ascendente Escorpio genera intensidad, reserva y una presencia magnetica.",
        [ZodiacSign.Sagittarius] = "Tu Ascendente Sagitario se siente expansivo, franco y orientado a descubrir.",
        [ZodiacSign.Capricorn] = "Tu Ascendente Capricornio proyecta madurez, prudencia y autoridad natural.",
        [ZodiacSign.Aquarius] = "Tu Ascendente Acuario se percibe singular, mental e independiente.",
        [ZodiacSign.Pisces] = "Tu Ascendente Piscis da una impresion suave, empatica e intuitiva."
    };

    public static readonly IReadOnlyDictionary<ZodiacSign, string> MercuryBySign = new Dictionary<ZodiacSign, string>
    {
        [ZodiacSign.Aries] = "Mercurio en Aries piensa rapido, comunica con franqueza y prefiere ir al punto.",
        [ZodiacSign.Taurus] = "Mercurio en Tauro procesa con calma, prioriza lo concreto y sostiene ideas con firmeza.",
        [ZodiacSign.Gemini] = "Mercurio en Geminis potencia agilidad mental, curiosidad y facilidad para conectar temas diversos.",
        [ZodiacSign.Cancer] = "Mercurio en Cancer comunica desde la memoria, la sensibilidad y la lectura emocional del contexto.",
        [ZodiacSign.Leo] = "Mercurio en Leo expresa ideas con calor, seguridad y un estilo personal marcado.",
        [ZodiacSign.Virgo] = "Mercurio en Virgo observa detalles, ordena informacion y busca precision al pensar y hablar.",
        [ZodiacSign.Libra] = "Mercurio en Libra razona comparando perspectivas y buscando equilibrio en el dialogo.",
        [ZodiacSign.Scorpio] = "Mercurio en Escorpio piensa en profundidad, detecta subtextos y comunica con intensidad.",
        [ZodiacSign.Sagittarius] = "Mercurio en Sagitario prioriza la vision general, el sentido y las ideas amplias.",
        [ZodiacSign.Capricorn] = "Mercurio en Capricornio organiza el pensamiento con estructura, realismo y estrategia.",
        [ZodiacSign.Aquarius] = "Mercurio en Acuario piensa de forma original, abstracta y poco convencional.",
        [ZodiacSign.Pisces] = "Mercurio en Piscis combina intuicion, imagen simbolica y sensibilidad para captar matices."
    };

    public static readonly IReadOnlyDictionary<ZodiacSign, string> VenusBySign = new Dictionary<ZodiacSign, string>
    {
        [ZodiacSign.Aries] = "Venus en Aries ama con impulso, iniciativa y gusto por lo vivo e intenso.",
        [ZodiacSign.Taurus] = "Venus en Tauro busca placer estable, lealtad y disfrute sensorial en los vinculos.",
        [ZodiacSign.Gemini] = "Venus en Geminis necesita conversacion, juego mental y variedad para interesarse afectivamente.",
        [ZodiacSign.Cancer] = "Venus en Cancer ama cuidando, conteniendo y construyendo intimidad emocional.",
        [ZodiacSign.Leo] = "Venus en Leo quiere afecto visible, generoso y una experiencia romantica con calidez.",
        [ZodiacSign.Virgo] = "Venus en Virgo demuestra amor en detalles, ayuda practica y atencion concreta.",
        [ZodiacSign.Libra] = "Venus en Libra valora armonia, reciprocidad y belleza en la forma de vincularse.",
        [ZodiacSign.Scorpio] = "Venus en Escorpio ama con intensidad, profundidad y fuerte necesidad de verdad emocional.",
        [ZodiacSign.Sagittarius] = "Venus en Sagitario necesita libertad, honestidad y expansion compartida en el amor.",
        [ZodiacSign.Capricorn] = "Venus en Capricornio vincula afecto con compromiso, seriedad y construccion a largo plazo.",
        [ZodiacSign.Aquarius] = "Venus en Acuario necesita espacio, autenticidad y un vinculo que respete la individualidad.",
        [ZodiacSign.Pisces] = "Venus en Piscis ama con ternura, idealismo y gran capacidad de fusion emocional."
    };

    public static readonly IReadOnlyDictionary<ZodiacSign, string> MarsBySign = new Dictionary<ZodiacSign, string>
    {
        [ZodiacSign.Aries] = "Marte en Aries actua con coraje, rapidez y fuerte impulso competitivo.",
        [ZodiacSign.Taurus] = "Marte en Tauro sostiene el esfuerzo con paciencia, resistencia y voluntad persistente.",
        [ZodiacSign.Gemini] = "Marte en Geminis se activa con ideas, variedad y movimiento mental constante.",
        [ZodiacSign.Cancer] = "Marte en Cancer actua desde lo emocional, la proteccion y la defensa de lo propio.",
        [ZodiacSign.Leo] = "Marte en Leo moviliza energia creativa, orgullo y deseo de expresarse con fuerza.",
        [ZodiacSign.Virgo] = "Marte en Virgo enfoca la accion en mejorar, resolver y ejecutar con precision.",
        [ZodiacSign.Libra] = "Marte en Libra busca actuar negociando, midiendo fuerzas y evitando choques innecesarios.",
        [ZodiacSign.Scorpio] = "Marte en Escorpio concentra poder, estrategia e intensidad para avanzar.",
        [ZodiacSign.Sagittarius] = "Marte en Sagitario impulsa accion franca, aventurera y orientada a metas amplias.",
        [ZodiacSign.Capricorn] = "Marte en Capricornio canaliza energia con disciplina, ambicion y eficacia.",
        [ZodiacSign.Aquarius] = "Marte en Acuario actua con independencia, innovacion y fuerte conviccion personal.",
        [ZodiacSign.Pisces] = "Marte en Piscis moviliza energia de forma sensible, adaptable e inspirada."
    };

    public static readonly IReadOnlyDictionary<int, string> HouseThemes = new Dictionary<int, string>
    {
        [1] = "La Casa 1 describe la forma en que inicias, te muestras y enfrentas el mundo.",
        [4] = "La Casa 4 habla de tus raices, intimidad, hogar emocional y sentido de pertenencia.",
        [7] = "La Casa 7 muestra como te vinculas, que buscas en pareja y como negocias con el otro.",
        [10] = "La Casa 10 refleja vocacion, imagen publica, ambicion y direccion de largo plazo."
    };

    public static readonly IReadOnlyDictionary<int, string> HouseTitles = new Dictionary<int, string>
    {
        [1] = "Identidad y presencia",
        [4] = "Raices y mundo privado",
        [7] = "Vinculos y pareja",
        [10] = "Vocacion y proyeccion publica"
    };
}
