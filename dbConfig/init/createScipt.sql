
-- Table: public.Stats

DROP TABLE IF EXISTS public."Stats";

CREATE TABLE IF NOT EXISTS public."Stats"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    wins integer NOT NULL DEFAULT 0,
    losses integer NOT NULL DEFAULT 0,
    eloscore integer NOT NULL DEFAULT 100,
    CONSTRAINT "Stats_pkey" PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Stats"
    OWNER to admin;


-- Table: public.User

DROP TABLE IF EXISTS public."User";

CREATE TABLE IF NOT EXISTS public."User"
(
    username text COLLATE pg_catalog."default" NOT NULL,
    password text COLLATE pg_catalog."default" NOT NULL,
    salt text COLLATE pg_catalog."default" NOT NULL,
    admin boolean NOT NULL DEFAULT 'false',
    coins integer NOT NULL DEFAULT 20,
    "statsID" integer NOT NULL,
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 23412 MINVALUE 23412 MAXVALUE 2147483647 CACHE 1 ),
    CONSTRAINT "User_pkey" PRIMARY KEY (id),
    CONSTRAINT stats FOREIGN KEY ("statsID")
        REFERENCES public."Stats" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."User"
    OWNER to admin;


-- Table: public.UserToken

DROP TABLE IF EXISTS public."UserToken";

CREATE TABLE IF NOT EXISTS public."UserToken"
(
    id integer NOT NULL,
    "userID" integer NOT NULL,
    "authToken" text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "UserToken_pkey" PRIMARY KEY (id),
    CONSTRAINT "user" FOREIGN KEY ("userID")
        REFERENCES public."User" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."UserToken"
    OWNER to admin;


-- Table: public.Card

DROP TABLE IF EXISTS public."Card";

CREATE TABLE IF NOT EXISTS public."Card"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    "cardID" text COLLATE pg_catalog."default" NOT NULL,
    "cardType" integer NOT NULL,
    "Name" text COLLATE pg_catalog."default" NOT NULL,
    "Damage" integer NOT NULL,
    "ElementType" integer NOT NULL,
    "MonsterType" integer,
    CONSTRAINT "Card_pkey" PRIMARY KEY (id),
    CONSTRAINT "cardID" UNIQUE ("cardID")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Card"
    OWNER to admin;


-- Table: public.Stacks

DROP TABLE IF EXISTS public."Stacks";

CREATE TABLE IF NOT EXISTS public."Stacks"
(
    "userID" integer NOT NULL,
    "cardID" integer NOT NULL,
    "isDeck" boolean NOT NULL DEFAULT 'false',
    CONSTRAINT pk_stacks PRIMARY KEY ("userID", "cardID"),
    CONSTRAINT "cardID" FOREIGN KEY ("cardID")
        REFERENCES public."Card" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT "userID" FOREIGN KEY ("userID")
        REFERENCES public."User" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Stacks"
    OWNER to admin;


INSERT INTO public."Stats"(
    id, wins, losses, eloscore)
	VALUES (1, 5, 0, 140);

INSERT INTO public."User"(
	username, password, salt, admin, "statsID")
	VALUES ("admin", "", "", 1, 1);