"use client";
import { vars } from "nativewind";

export const config = {
  light: vars({
    /* Primary - Green */
    "--color-primary-0": "245 252 238", // #f5fcee
    "--color-primary-50": "235 248 221", // #ebf8dd
    "--color-primary-100": "215 241 188", // #d7f1bc
    "--color-primary-200": "185 230 138", // #b9e68a
    "--color-primary-300": "150 215 85", // #96d755
    "--color-primary-400": "135 205 58", // #87cd3a
    "--color-primary-500": "120 190 32", // #78be20 ← Base color
    "--color-primary-600": "100 160 25", // #64a019
    "--color-primary-700": "80 130 20", // #508214
    "--color-primary-800": "60 95 15", // #3c5f0f
    "--color-primary-900": "40 65 10", // #28410a
    "--color-primary-950": "25 40 6", // #192806

    /* Secondary - Blue */
    "--color-secondary-0": "240 247 252", // #f0f7fc
    "--color-secondary-50": "230 242 250", // #e6f2fa
    "--color-secondary-100": "210 234 247", // #d2eaf7
    "--color-secondary-200": "170 218 241", // #aadaf1
    "--color-secondary-300": "130 200 235", // #82c8eb
    "--color-secondary-400": "119 186 234", // #77baea
    "--color-secondary-500": "108 172 228", // #6cace4 ← Base color
    "--color-secondary-600": "86 138 182", // #568ab6
    "--color-secondary-700": "65 103 137", // #416789
    "--color-secondary-800": "43 69 91", // #2b455b
    "--color-secondary-900": "32 52 68", // #203444
    "--color-secondary-950": "22 34 46", // #16222e

    /* Tertiary - Purple */
    "--color-tertiary-0": "250 238 245", // #faeef5
    "--color-tertiary-50": "245 220 235", // #f5dceb
    "--color-tertiary-100": "235 190 215", // #ebbed7
    "--color-tertiary-200": "215 140 185", // #d78cb9
    "--color-tertiary-300": "195 90 155", // #c35a9b
    "--color-tertiary-400": "174 54 128", // #ae3680
    "--color-tertiary-500": "153 30 102", // #991e66 ← Base color
    "--color-tertiary-600": "122 24 82", // #7a1852
    "--color-tertiary-700": "92 18 61", // #5c123d
    "--color-tertiary-800": "61 12 41", // #3d0c29
    "--color-tertiary-900": "46 9 31", // #2e091f
    "--color-tertiary-950": "31 6 20", // #1f0614

    /* Error - Red */
    "--color-error-0": "255 245 242", // #fff5f2
    "--color-error-50": "255 235 230", // #ffebe6
    "--color-error-100": "255 215 205", // #ffd7cd
    "--color-error-200": "255 175 155", // #ffaf9b
    "--color-error-300": "255 135 105", // #ff8769
    "--color-error-400": "255 98 72", // #ff6248
    "--color-error-500": "255 92 57", // #ff5c39 ← Base color
    "--color-error-600": "204 74 46", // #cc4a2e
    "--color-error-700": "153 55 34", // #993722
    "--color-error-800": "102 37 23", // #662517
    "--color-error-900": "76 28 17", // #4c1c11
    "--color-error-950": "51 18 11", // #33120b

    /* Success */
    "--color-success-0": "228 255 244",
    "--color-success-50": "202 255 232",
    "--color-success-100": "162 241 192",
    "--color-success-200": "132 211 162",
    "--color-success-300": "102 181 132",
    "--color-success-400": "72 151 102",
    "--color-success-500": "52 131 82",
    "--color-success-600": "42 121 72",
    "--color-success-700": "32 111 62",
    "--color-success-800": "22 101 52",
    "--color-success-900": "20 83 45",
    "--color-success-950": "27 50 36",

    /* Warning - Yellow */
    "--color-warning-0": "255 252 242", // #fffcf2
    "--color-warning-50": "255 249 230", // #fff9e6
    "--color-warning-100": "255 243 205", // #fff3cd
    "--color-warning-200": "255 231 155", // #ffe79b
    "--color-warning-300": "255 219 105", // #ffdb69
    "--color-warning-400": "255 211 68", // #ffd344
    "--color-warning-500": "255 199 44", // #ffc72c ← Base color
    "--color-warning-600": "204 159 35", // #cc9f23
    "--color-warning-700": "153 119 26", // #99771a
    "--color-warning-800": "102 80 18", // #665012
    "--color-warning-900": "76 60 13", // #4c3c0d
    "--color-warning-950": "51 40 9", // #332809

    /* Info */
    "--color-info-0": "236 248 254",
    "--color-info-50": "199 235 252",
    "--color-info-100": "162 221 250",
    "--color-info-200": "124 207 248",
    "--color-info-300": "87 194 246",
    "--color-info-400": "50 180 244",
    "--color-info-500": "13 166 242",
    "--color-info-600": "11 141 205",
    "--color-info-700": "9 115 168",
    "--color-info-800": "7 90 131",
    "--color-info-900": "5 64 93",
    "--color-info-950": "3 38 56",

    /* Typography */
    "--color-typography-0": "254 254 255",
    "--color-typography-50": "245 245 245",
    "--color-typography-100": "229 229 229",
    "--color-typography-200": "219 219 220",
    "--color-typography-300": "212 212 212",
    "--color-typography-400": "163 163 163",
    "--color-typography-500": "140 140 140",
    "--color-typography-600": "115 115 115",
    "--color-typography-700": "82 82 82",
    "--color-typography-800": "64 64 64",
    "--color-typography-900": "38 38 39",
    "--color-typography-950": "23 23 23",

    /* Outline */
    "--color-outline-0": "253 254 254",
    "--color-outline-50": "243 243 243",
    "--color-outline-100": "230 230 230",
    "--color-outline-200": "221 220 219",
    "--color-outline-300": "211 211 211",
    "--color-outline-400": "165 163 163",
    "--color-outline-500": "140 141 141",
    "--color-outline-600": "115 116 116",
    "--color-outline-700": "83 82 82",
    "--color-outline-800": "65 65 65",
    "--color-outline-900": "39 38 36",
    "--color-outline-950": "26 23 23",

    /* Background */
    "--color-background-0": "255 255 255", // #ffffff
    "--color-background-50": "248 249 250", // #f8f9fa
    "--color-background-100": "240 242 244", // #f0f2f4
    "--color-background-200": "220 225 228", // #dce1e4
    "--color-background-300": "180 190 196", // #b4bec4
    "--color-background-400": "140 155 164", // #8c9ba4
    "--color-background-500": "100 120 132", // #647884
    "--color-background-600": "80 100 112", // #506470
    "--color-background-700": "65 82 92", // #41525c
    "--color-background-800": "51 63 72", // #333f48 ← Base dark background
    "--color-background-900": "38 47 54", // #262f36
    "--color-background-950": "25 31 36", // #191f24

    /* Background Special */
    "--color-background-error": "254 241 241",
    "--color-background-warning": "255 243 234",
    "--color-background-success": "237 252 242",
    "--color-background-muted": "247 248 247",
    "--color-background-info": "235 248 254",

    /* Focus Ring Indicator  */
    "--color-indicator-primary": "55 55 55",
    "--color-indicator-info": "83 153 236",
    "--color-indicator-error": "185 28 28",
  }),
  dark: vars({
    /* Primary - Green (#78BE20) - Brighter for dark backgrounds */
    "--color-primary-0": "25 40 6",
    "--color-primary-50": "40 65 10",
    "--color-primary-100": "60 95 15",
    "--color-primary-200": "80 130 20",
    "--color-primary-300": "100 160 25",
    "--color-primary-400": "120 190 32",
    "--color-primary-500": "135 205 58",
    "--color-primary-600": "150 215 85",
    "--color-primary-700": "185 230 138",
    "--color-primary-800": "215 241 188",
    "--color-primary-900": "235 248 221",
    "--color-primary-950": "245 252 238",

    /* Secondary - Blue (#6CACE4) - Brighter for dark backgrounds */
    "--color-secondary-0": "22 34 46",
    "--color-secondary-50": "32 52 68",
    "--color-secondary-100": "43 69 91",
    "--color-secondary-200": "65 103 137",
    "--color-secondary-300": "86 138 182",
    "--color-secondary-400": "108 172 228",
    "--color-secondary-500": "119 186 234",
    "--color-secondary-600": "130 200 235",
    "--color-secondary-700": "170 218 241",
    "--color-secondary-800": "210 234 247",
    "--color-secondary-900": "230 242 250",
    "--color-secondary-950": "240 247 252",

    /* Tertiary - Purple (#991E66) - Brighter for dark backgrounds */
    "--color-tertiary-0": "31 6 20",
    "--color-tertiary-50": "46 9 31",
    "--color-tertiary-100": "61 12 41",
    "--color-tertiary-200": "92 18 61",
    "--color-tertiary-300": "122 24 82",
    "--color-tertiary-400": "153 30 102",
    "--color-tertiary-500": "174 54 128",
    "--color-tertiary-600": "195 90 155",
    "--color-tertiary-700": "215 140 185",
    "--color-tertiary-800": "235 190 215",
    "--color-tertiary-900": "245 220 235",
    "--color-tertiary-950": "250 238 245",

    /* Error - Red (#FF5C39) - Brighter for dark backgrounds */
    "--color-error-0": "51 18 11",
    "--color-error-50": "76 28 17",
    "--color-error-100": "102 37 23",
    "--color-error-200": "153 55 34",
    "--color-error-300": "204 74 46",
    "--color-error-400": "255 92 57",
    "--color-error-500": "255 98 72",
    "--color-error-600": "255 135 105",
    "--color-error-700": "255 175 155",
    "--color-error-800": "255 215 205",
    "--color-error-900": "255 235 230",
    "--color-error-950": "255 245 242",

    /* Success */
    "--color-success-0": "27 50 36",
    "--color-success-50": "20 83 45",
    "--color-success-100": "22 101 52",
    "--color-success-200": "32 111 62",
    "--color-success-300": "42 121 72",
    "--color-success-400": "52 131 82",
    "--color-success-500": "72 151 102",
    "--color-success-600": "102 181 132",
    "--color-success-700": "132 211 162",
    "--color-success-800": "162 241 192",
    "--color-success-900": "202 255 232",
    "--color-success-950": "228 255 244",

    /* Warning - Yellow (#FFC72C) - Brighter for dark backgrounds */
    "--color-warning-0": "51 40 9",
    "--color-warning-50": "76 60 13",
    "--color-warning-100": "102 80 18",
    "--color-warning-200": "153 119 26",
    "--color-warning-300": "204 159 35",
    "--color-warning-400": "255 199 44",
    "--color-warning-500": "255 211 68",
    "--color-warning-600": "255 219 105",
    "--color-warning-700": "255 231 155",
    "--color-warning-800": "255 243 205",
    "--color-warning-900": "255 249 230",
    "--color-warning-950": "255 252 242",

    /* Info */
    "--color-info-0": "3 38 56",
    "--color-info-50": "5 64 93",
    "--color-info-100": "7 90 131",
    "--color-info-200": "9 115 168",
    "--color-info-300": "11 141 205",
    "--color-info-400": "13 166 242",
    "--color-info-500": "50 180 244",
    "--color-info-600": "87 194 246",
    "--color-info-700": "124 207 248",
    "--color-info-800": "162 221 250",
    "--color-info-900": "199 235 252",
    "--color-info-950": "236 248 254",

    /* Typography */
    "--color-typography-0": "23 23 23",
    "--color-typography-50": "38 38 39",
    "--color-typography-100": "64 64 64",
    "--color-typography-200": "82 82 82",
    "--color-typography-300": "115 115 115",
    "--color-typography-400": "140 140 140",
    "--color-typography-500": "163 163 163",
    "--color-typography-600": "212 212 212",
    "--color-typography-700": "219 219 220",
    "--color-typography-800": "229 229 229",
    "--color-typography-900": "245 245 245",
    "--color-typography-950": "254 254 255",

    /* Outline */
    "--color-outline-0": "26 23 23",
    "--color-outline-50": "39 38 36",
    "--color-outline-100": "65 65 65",
    "--color-outline-200": "83 82 82",
    "--color-outline-300": "115 116 116",
    "--color-outline-400": "140 141 141",
    "--color-outline-500": "165 163 163",
    "--color-outline-600": "211 211 211",
    "--color-outline-700": "221 220 219",
    "--color-outline-800": "230 230 230",
    "--color-outline-900": "243 243 243",
    "--color-outline-950": "253 254 254",

    /* Background - Darker for dark mode */
    "--color-background-0": "25 31 36",
    "--color-background-50": "38 47 54",
    "--color-background-100": "51 63 72",
    "--color-background-200": "65 82 92",
    "--color-background-300": "80 100 112",
    "--color-background-400": "100 120 132",
    "--color-background-500": "140 155 164",
    "--color-background-600": "180 190 196",
    "--color-background-700": "220 225 228",
    "--color-background-800": "240 242 244",
    "--color-background-900": "248 249 250",
    "--color-background-950": "255 255 255",

    /* Background Special */
    "--color-background-error": "66 43 43",
    "--color-background-warning": "65 47 35",
    "--color-background-success": "28 43 33",
    "--color-background-muted": "51 51 51",
    "--color-background-info": "26 40 46",

    /* Focus Ring Indicator  */
    "--color-indicator-primary": "247 247 247",
    "--color-indicator-info": "161 199 245",
    "--color-indicator-error": "232 70 69",
  }),
};
