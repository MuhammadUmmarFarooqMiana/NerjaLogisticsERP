import { Box } from '@mui/material';
import logoBlack from '../../assets/Nerja logo Black.png';
import logoWhite from '../../assets/Nerja logoWhite.png';

interface LogoProps {
  height?: number;
  variant?: 'black' | 'white';
}

const ASPECT_RATIO = 366 / 58;

export function Logo({ height = 24, variant = 'black' }: LogoProps) {
  const src = variant === 'white' ? logoWhite : logoBlack;
  return (
    <Box
      component="img"
      src={src}
      alt="Nerja Logistics ERP"
      height={height}
      width={height * ASPECT_RATIO}
      sx={{ display: 'block', objectFit: 'contain' }}
    />
  );
}
