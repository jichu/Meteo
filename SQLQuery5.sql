﻿Select MODELS.ID, count(*) pocet from MODELS JOIN MASK_SPECTRUM ON MODELS.ID = MASK_SPECTRUM.ID GROUP BY MODELS.ID