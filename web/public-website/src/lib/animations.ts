export const EASING = {
  smooth: [0.4, 0, 0.2, 1] as const,
  ease:   [0.25, 0.46, 0.45, 0.94] as const,
}

export const TRANSITIONS = {
  fast:   { duration: 0.15, ease: EASING.smooth },
  normal: { duration: 0.2,  ease: EASING.smooth },
  slow:   { duration: 0.3,  ease: EASING.smooth },
  slower: { duration: 0.5,  ease: EASING.smooth },
}

export const fadeInUp = {
  hidden:  { opacity: 0, y: 20 },
  visible: { opacity: 1, y: 0, transition: TRANSITIONS.normal },
}

export const fadeIn = {
  hidden:  { opacity: 0 },
  visible: { opacity: 1, transition: TRANSITIONS.slow },
}

export const staggerContainer = {
  hidden:  { opacity: 0 },
  visible: {
    opacity: 1,
    transition: { staggerChildren: 0.1, delayChildren: 0.15 },
  },
}

export const staggerItem = {
  hidden:  { opacity: 0, y: 24 },
  visible: { opacity: 1, y: 0, transition: TRANSITIONS.normal },
}

export const hoverLift = {
  whileHover: { y: -4, transition: TRANSITIONS.fast },
}

export const scaleIn = {
  initial:  { opacity: 0, scale: 0.95 },
  animate:  { opacity: 1, scale: 1, transition: TRANSITIONS.normal },
  exit:     { opacity: 0, scale: 0.95, transition: TRANSITIONS.fast },
}

export const slideInLeft = {
  hidden:  { opacity: 0, x: -30 },
  visible: { opacity: 1, x: 0, transition: TRANSITIONS.slow },
}

export const slideInRight = {
  hidden:  { opacity: 0, x: 30 },
  visible: { opacity: 1, x: 0, transition: TRANSITIONS.slow },
}
