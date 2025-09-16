import React, { useState, useEffect } from 'react';

function ImageWithFallback({
    src,
    alt,
    fallbackSrc = '/images/placeholder.png',
    className = 'img',
    style = {}
}) {
    const [imageSrc, setImageSrc] = useState(src || fallbackSrc); // Use fallbackSrc if src is empty

    useEffect(() => {
        if (!src) {
            setImageSrc(fallbackSrc); // Set fallback if src is empty
        } else {
            setImageSrc(src);
        }
    }, [src, fallbackSrc]);

    const handleError = () => {
        if (imageSrc !== fallbackSrc) { // Prevent unnecessary fallback
            setImageSrc(fallbackSrc);
        }
    };

    return (
        <img
            src={imageSrc}
            alt={alt}
            onError={handleError}
            className={className}
            style={style}
        />
    );
}

export default ImageWithFallback;
