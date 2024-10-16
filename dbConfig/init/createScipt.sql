
-- Table: public.Stats

DROP TABLE IF EXISTS public."Stats";

CREATE TABLE IF NOT EXISTS public."Stats"
(
    id integer NOT NULL,
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
    deck text[] COLLATE pg_catalog."default",
    stack text[] COLLATE pg_catalog."default",
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 23412 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
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